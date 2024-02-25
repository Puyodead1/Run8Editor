using Toolbox.Core;

namespace Run8Library
{
    /// <summary>
    /// Represents a plugin for a map editor.
    /// This is required for every dll so the tool knows it is a valid plugin to use.
    /// </summary>
    public class Plugin : IPlugin
    {
        public string Name => "Run8 Editor";

        public Plugin()
        {
        }
    }
}
