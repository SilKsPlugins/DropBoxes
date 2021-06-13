using Cysharp.Threading.Tasks;
using DropBoxes.API;
using OpenMod.API.Commands;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Users;
using System;

namespace DropBoxes.Commands
{
    [Command("unbox", Priority = Priority.High)]
    [CommandSyntax("<box>")]
    [CommandDescription("Unbox a loot box.")]
    [CommandActor(typeof(UnturnedUser))]
    public class CUnbox : LootBoxCommand
    {
        private readonly ILootBoxManager _lootBoxManager;
        private readonly IServiceProvider _serviceProvider;

        public CUnbox(ILootBoxManager lootBoxManager,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _lootBoxManager = lootBoxManager;
            _serviceProvider = serviceProvider;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var user = (UnturnedUser) Context.Actor;

            var lootBoxAsset = await GetLootBoxAsset(0);

            var reward = lootBoxAsset.SelectReward();

            if (!await _lootBoxManager.RemoveLootBox(user.SteamId.m_SteamID, lootBoxAsset))
            {
                throw new UserFriendlyException(StringLocalizer["Commands:Errors:NoLootBox",
                    new {LootBox = lootBoxAsset}]);
            }

            var itemInstance = await reward.GiveUser(user, _serviceProvider) ??
                               throw new Exception(
                                   $"Could not give user {user.SteamId.m_SteamID} loot box {lootBoxAsset.BoxId}.");

            await PrintAsync(
                StringLocalizer["Commands:Success:Unbox", new {itemInstance.Item, LootBox = lootBoxAsset}]);
        }
    }
}
