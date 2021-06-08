using OpenMod.API.Plugins;
using OpenMod.Unturned.Plugins;
using System;

[assembly: PluginMetadata("DropBoxes", DisplayName = "Drop Boxes", Author = "SilK")]
namespace DropBoxes
{
    public class DropBoxesPlugin : OpenModUnturnedPlugin
    {
        public DropBoxesPlugin(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
