using Cysharp.Threading.Tasks;
using DropBoxes.API;
using DropBoxes.Chat;
using DropBoxes.Configuration;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Users;
using SilK.Unturned.Extras.Configuration;
using Steamworks;
using System;

namespace DropBoxes.Commands
{
    [Command("giveboxbyid", Priority = Priority.High)]
    [CommandSyntax("<steam id> <box>")]
    [CommandDescription("Give the player with the steam ID a loot box.")]
    public class CGiveBoxById : LootBoxCommand
    {
        private readonly ILootBoxManager _lootBoxManager;
        private readonly IUnturnedUserDirectory _unturnedUserDirectory;
        private readonly IConfigurationParser<DropBoxesConfiguration> _configuration;

        public CGiveBoxById(IServiceProvider serviceProvider,
            ILootBoxManager lootBoxManager,
            IUnturnedUserDirectory unturnedUserDirectory,
            IConfigurationParser<DropBoxesConfiguration> configuration) : base(serviceProvider)
        {
            _lootBoxManager = lootBoxManager;
            _unturnedUserDirectory = unturnedUserDirectory;
            _configuration = configuration;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var steamId = await Context.Parameters.GetAsync<ulong>(0);
            var lootBoxAsset = await GetLootBoxAsset(1);

            var user = _unturnedUserDirectory.FindUser(new CSteamID(steamId));

            await _lootBoxManager.GiveLootBox(steamId, lootBoxAsset);

            await this.PrintMessageWithIconAsync(StringLocalizer["Commands:Success:GiveBoxById",
                new {SteamId = steamId, LootBox = lootBoxAsset}], _configuration.Instance.IconUrl);

            if (user != null)
            {
                await user.PrintMessageWithIconAsync(StringLocalizer["Commands:Success:ReceivedBox",
                    new {LootBox = lootBoxAsset}], _configuration.Instance.IconUrl);
            }
        }
    }
}
