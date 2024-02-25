using GLFrameworkEngine;
using LibRun8.Common;
using LibRun8.Formats;
using MapStudio.UI;
using Toolbox.Core;
using Toolbox.Core.ViewModels;
using static Toolbox.Core.DDS;

namespace Run8Library.Files
{
    /// <summary>
    /// Represents a class used for loading files into the editor.
    /// IFileFormat determines what files to use. FileEditor is used to store all the editor information.
    /// </summary>
    public class RN8 : FileEditor, IFileFormat
    {
        List<NodeBase> Nodes = new List<NodeBase>();

        /// <summary>
        /// The description of the file extension of the plugin.
        /// </summary>
        string[] IFileFormat.Description => new string[] { "Run8 Model" };

        /// <summary>
        /// The extension of the plugin. This should match whatever file you plan to open.
        /// </summary>
        string[] IFileFormat.Extension => new string[] { "*.rn8" };

        /// <summary>
        /// Determines if the plugin can save or not.
        /// </summary>
        bool IFileFormat.CanSave { get; set; } = false;

        /// <summary>
        /// File info of the loaded file format.
        /// </summary>
        File_Info IFileFormat.FileInfo { get; set; }

        /// <summary>
        /// Determines when to use the map editor from a given file.
        /// You can check from file extension or check the data inside the file stream.
        /// The file stream is always decompressed if the given file has a supported ICompressionFormat like Yaz0.
        /// </summary>
        bool IFileFormat.Identify(File_Info fileInfo, Stream stream)
        {
            // reset stream position
            stream.Position = 0;
            return true;
        }

        /// <summary>
        /// Loads the given file data from a stream.
        /// </summary>
        void IFileFormat.Load(Stream stream)
        {
            //A folder to represent in the outliner UI
            NodeBase folder = new NodeBase("Objects");
            //Allow toggling visibility for the folder
            folder.HasCheckBox = true;
            //Add it to the root of our loader
            //It is important you use "AddChild" so the parent is applied
            Root.AddChild(folder);
            //Icons can be obtained from the icon manager constants
            //These also are all from font awesome and can be used directly
            folder.Icon = IconManager.MODEL_ICON.ToString();

            Model model = Model.Read(stream);

            foreach (ModelObject obj in model.Objects)
            {
                NodeBase parentNode = folder;

                if(!string.IsNullOrEmpty(obj.ParentName))
                {
                    // find the parent node
                    parentNode = Nodes.Find(x => x.Header == obj.ParentName);
                    if(parentNode == null)
                    {
                        StudioLogger.WriteWarning("Failed to find parent node " + obj.ParentName);
                        parentNode = folder;
                    }
                }

                ModelRenderer renderer = new ModelRenderer(obj, parentNode);
                renderer.UINode.Icon = IconManager.MESH_ICON.ToString();
                if (model.ObjectCount == 1 || string.IsNullOrEmpty(obj.Name))
                {
                    renderer.UINode.Header = "Model";
                }
                else
                {
                    renderer.UINode.Header = obj.Name;
                }

                renderer.Transform.Position = new OpenTK.Vector3(obj.Position.X, obj.Position.Y, obj.Position.Z);
                //renderer.Transform.Scale = new OpenTK.Vector3(1.0f);
                //renderer.Transform.Rotation = new OpenTK.Quaternion(obj.UnkQuat2.X, obj.UnkQuat2.Y, obj.UnkQuat2.Z, obj.UnkQuat2.W);
                renderer.Transform.TransformMatrix = new OpenTK.Matrix4(
                    model.UnkMatrix.M11, model.UnkMatrix.M12, model.UnkMatrix.M13, model.UnkMatrix.M14,
                    model.UnkMatrix.M21, model.UnkMatrix.M22, model.UnkMatrix.M23, model.UnkMatrix.M24,
                    model.UnkMatrix.M31, model.UnkMatrix.M32, model.UnkMatrix.M33, model.UnkMatrix.M34,
                    model.UnkMatrix.M41, model.UnkMatrix.M42, model.UnkMatrix.M43, model.UnkMatrix.M44);
                renderer.Transform.UpdateMatrix(true);

                Nodes.Add(renderer.UINode);
                AddRender(renderer);
            }
        }

        /// <summary>
        /// Saves the given file data to a stream.
        /// </summary>
        void IFileFormat.Save(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
