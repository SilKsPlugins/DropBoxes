extern alias JetBrainsAnnotations;
using DropBoxes.LootBoxes;
using JetBrainsAnnotations::JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using OpenMod.API.Ioc;
using OpenMod.Core.Commands;

namespace DropBoxes
{
    [UsedImplicitly]
    public class ServiceConfigurator : IServiceConfigurator
    {
        public void ConfigureServices(IOpenModServiceConfigurationContext openModStartupContext,
            IServiceCollection serviceCollection)
        {
            serviceCollection.Configure<CommandParameterResolverOptions>(options =>
            {
                options.AddCommandParameterResolveProvider<LootBoxAssetCommandParameterResolveProvider>();
            });
        }
    }
}
