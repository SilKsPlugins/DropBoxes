extern alias JetBrainsAnnotations;
using Autofac;
using JetBrainsAnnotations::JetBrains.Annotations;
using OpenMod.API.Ioc;
using SilK.OpenMod.EntityFrameworkCore;

namespace DropBoxes
{
    [UsedImplicitly]
    public class ContainerConfigurator : IContainerConfigurator
    {
        public void ConfigureContainer(IOpenModServiceConfigurationContext openModStartupContext, ContainerBuilder containerBuilder)
        {
            containerBuilder.AddPomeloMySqlConnectorResolver();
        }
    }
}
