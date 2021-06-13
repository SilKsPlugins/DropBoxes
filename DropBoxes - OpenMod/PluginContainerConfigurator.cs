extern alias JetBrainsAnnotations;
using DropBoxes.Database;
using JetBrainsAnnotations::JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Plugins;
using OpenMod.EntityFrameworkCore.Extensions;

namespace DropBoxes
{
    [UsedImplicitly]
    public class PluginContainerConfigurator : IPluginContainerConfigurator
    {
        public void ConfigureContainer(IPluginServiceConfigurationContext context)
        {
            context.ContainerBuilder.AddEntityFrameworkCoreMySql();
            context.ContainerBuilder.AddDbContext<DropBoxesDbContext>(ServiceLifetime.Transient);
        }
    }
}
