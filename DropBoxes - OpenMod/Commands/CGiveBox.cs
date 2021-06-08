extern alias JetBrainsAnnotations;
using Cysharp.Threading.Tasks;
using DropBoxes.API;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Users;
using System;

namespace DropBoxes.Commands
{
    [Command("givebox", Priority = Priority.High)]
    [CommandSyntax("<player> <box>")]
    [CommandDescription("Give an online player a loot box.")]
    public class CGiveBox : LootBoxCommand
    {
        private readonly ILootBoxManager _lootBoxManager;

        public CGiveBox(ILootBoxManager lootBoxManager,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _lootBoxManager = lootBoxManager;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var user = await Context.Parameters.GetAsync<UnturnedUser>(0);
            var lootBoxAsset = await GetLootBoxAsset(1);

            await _lootBoxManager.GiveLootBox(user.SteamId.m_SteamID, lootBoxAsset);

            await PrintAsync(StringLocalizer["Commands:Success:GiveBox", new {User = user, LootBox = lootBoxAsset}]);

            await user.PrintMessageAsync(StringLocalizer["Commands:Success:ReceivedBox", new {LootBox = lootBoxAsset}]);
        }
    }
}
