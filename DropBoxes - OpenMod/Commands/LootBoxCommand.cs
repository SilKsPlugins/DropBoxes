using DropBoxes.Configuration;
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

        protected LootBoxCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            StringLocalizer = serviceProvider.GetRequiredService<IStringLocalizer>();
        }

        protected async Task<LootBoxAsset> GetLootBoxAsset(int index)
        {
            try
            {
                return await Context.Parameters.GetAsync<LootBoxAsset>(index);
            }
            catch (CommandParameterParseException)
            {
                throw new UserFriendlyException(StringLocalizer["Commands:Errors:UnknownLootBox",
                    new { Input = await Context.Parameters.GetAsync<string>(index) }]);
            }
        }
    }
}
