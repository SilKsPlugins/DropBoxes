extern alias JetBrainsAnnotations;
using DropBoxes.Database;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.API.Plugins;
using OpenMod.EntityFrameworkCore.MySql.Extensions;

namespace DropBoxes
{
    [UsedImplicitly]
    public class PluginContainerConfigurator : IPluginContainerConfigurator
    {
        public void ConfigureContainer(IPluginServiceConfigurationContext context)
        {
            context.ContainerBuilder.AddMySqlDbContext<DropBoxesDbContext>();
        }
    }
}
