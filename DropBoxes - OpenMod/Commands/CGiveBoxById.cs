using Cysharp.Threading.Tasks;
using DropBoxes.API;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Users;
using Steamworks;
using System;

namespace DropBoxes.Commands
{
    [Command("giveboxbyid", Priority = Priority.High)]
    [CommandSyntax("<player id> <box>")]
    [CommandDescription("Give the player with the ID a loot box.")]
    public class CGiveBoxById : LootBoxCommand
    {
        private readonly ILootBoxManager _lootBoxManager;
        private readonly IUnturnedUserDirectory _unturnedUserDirectory;

        public CGiveBoxById(ILootBoxManager lootBoxManager,
            IUnturnedUserDirectory unturnedUserDirectory,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _lootBoxManager = lootBoxManager;
            _unturnedUserDirectory = unturnedUserDirectory;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var steamId = await Context.Parameters.GetAsync<ulong>(0);
            var lootBoxAsset = await GetLootBoxAsset(1);

            var user = _unturnedUserDirectory.FindUser(new CSteamID(steamId));

            await _lootBoxManager.GiveLootBox(steamId, lootBoxAsset);

            await PrintAsync(StringLocalizer["Commands:Success:GiveBoxById",
                new {SteamId = steamId, LootBox = lootBoxAsset}]);

            if (user != null)
            {
                await user.PrintMessageAsync(StringLocalizer["Commands:Success:ReceivedBox",
                    new {LootBox = lootBoxAsset}]);
            }
        }
    }
}
