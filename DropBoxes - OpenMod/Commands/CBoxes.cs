extern alias JetBrainsAnnotations;
using Cysharp.Threading.Tasks;
using DropBoxes.API;
using Microsoft.Extensions.Localization;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using System;
using System.Linq;

namespace DropBoxes.Commands
{
    [Command("boxes", Priority = Priority.High)]
    [CommandDescription("View the loot boxes you have access to.")]
    [CommandActor(typeof(UnturnedUser))]
    public class CBoxes : UnturnedCommand
    {
        private readonly ILootBoxManager _lootBoxManager;
        private readonly IStringLocalizer _stringLocalizer;
        private readonly IServiceProvider _serviceProvider;

        public CBoxes(ILootBoxManager lootBoxManager,
            IStringLocalizer stringLocalizer,
            IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _lootBoxManager = lootBoxManager;
            _stringLocalizer = stringLocalizer;
            _serviceProvider = serviceProvider;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var user = (UnturnedUser) Context.Actor;

            var lootBoxes =
                (await _lootBoxManager.GetLootBoxes(user)).Select(x => new
                {
                    Instance = x,
                    Asset = x.GetAsset(_serviceProvider)
                }).Where(x => x.Asset != null)
                .Select(x => new
                {
                    x.Asset!.BoxId,
                    x.Asset!.Name,
                    x.Instance.Amount
                }).ToList();

            await PrintAsync(_stringLocalizer["Commands:Success:LootBoxes", new {LootBoxes = lootBoxes}]);
        }
    }
}
