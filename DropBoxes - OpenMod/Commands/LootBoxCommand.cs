using DropBoxes.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using OpenMod.API.Commands;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using System;
using System.Threading.Tasks;

namespace DropBoxes.Commands
{
    public abstract class LootBoxCommand : UnturnedCommand
    {
        protected readonly IStringLocalizer StringLocalizer;

        private readonly IConfiguration _configuration;

        protected LootBoxCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            StringLocalizer = serviceProvider.GetRequiredService<IStringLocalizer>();
            _configuration = serviceProvider.GetRequiredService<IConfiguration>();
        }

        protected async Task<LootBoxAsset> GetLootBoxAsset(int index)
        {
            if (Context.Parameters.Length <= index)
            {
                throw new CommandWrongUsageException(Context);
            }

            var input = await Context.Parameters.GetAsync<string>(index);

            var configuration = _configuration.Get<DropBoxesConfiguration>();

            return configuration.GetLootBoxAssetById(input) ??
                   configuration.GetLootBoxAssetByName(input) ??
                   throw new UserFriendlyException(StringLocalizer["Commands:Errors:UnknownLootBox",
                       new {Input = input}]);
        }
    }
}
