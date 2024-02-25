using GLFrameworkEngine;
using LibRun8.Common;
using Toolbox.Core.ViewModels;

namespace Run8Library.Files
{
    /// <summary>
    /// Represents a custom renderer that can be transformed and manipulated.
    /// </summary>
    public class ModelRenderer : EditableObject
    {
        public static OpenTK.Vector4 AnimatedColor = new OpenTK.Vector4(1);

        RenderMesh<VertexPositionNormalTexCoord> Mesh;
        StandardMaterial Material;

        public ModelRenderer(ModelObject obj, NodeBase parent = null) : base(parent)
        {

            List<VertexPositionNormalTexCoord> vertices = new List<VertexPositionNormalTexCoord>();

            foreach (VertexStruct libVert in obj.Vertices)
            {
                // convert librun8 vertex to toolbox vertex
                VertexPositionNormalTexCoord msVertex = new VertexPositionNormalTexCoord();
                msVertex.Position = new OpenTK.Vector3(libVert.Position.X, libVert.Position.Y, libVert.Position.Z);
                msVertex.Normal = new OpenTK.Vector3(libVert.Normal.X, libVert.Normal.Y, libVert.Normal.Z);
                msVertex.TexCoord = new OpenTK.Vector2(libVert.TextureCoordinate.X, libVert.TextureCoordinate.Y);

                vertices.Add(msVertex);
            }

            // create a new mesh object
            Mesh = new RenderMesh<VertexPositionNormalTexCoord>(vertices.ToArray(), obj.Indices.ToArray(), OpenTK.Graphics.OpenGL.PrimitiveType.Triangles);

            //The gl framework includes some base materials to easily use
            Material = new StandardMaterial();
            //We can also apply some in engine textures
            Material.DiffuseTextureID = RenderTools.uvTestPattern.ID;
        }

        public override void DrawModel(GLContext context, Pass pass)
        {
            //Make sure to draw on the right pass!
            //These are used to sort out transparent ordering
            if (pass == Pass.OPAQUE)
                DrawOpaque(context);
        }

        private void DrawOpaque(GLContext context)
        {
            //Apply material
            Material.ModelMatrix = this.Transform.TransformMatrix;
            Material.Color = AnimatedColor;
            //Disable textures for animated colors if used
            if (AnimatedColor != OpenTK.Vector4.One)
                Material.DiffuseTextureID = -1;

            Material.Render(context);
            //Draw with a selection visual. 
            Mesh.Draw(context);
        }
    }
}
