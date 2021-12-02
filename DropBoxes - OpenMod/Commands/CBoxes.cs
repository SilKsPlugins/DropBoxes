extern alias JetBrainsAnnotations;
using Cysharp.Threading.Tasks;
using DropBoxes.API;
using DropBoxes.Chat;
using DropBoxes.Configuration;
using Microsoft.Extensions.Localization;
using OpenMod.API.Prioritization;
using OpenMod.Core.Commands;
using OpenMod.Unturned.Commands;
using OpenMod.Unturned.Users;
using SilK.Unturned.Extras.Configuration;
using System;
using System.Linq;

namespace DropBoxes.Commands
{
    [Command("boxes", Priority = Priority.High)]
    [CommandDescription("View your loot boxes.")]
    [CommandActor(typeof(UnturnedUser))]
    public class CBoxes : UnturnedCommand
    {
        private readonly ILootBoxManager _lootBoxManager;
        private readonly IStringLocalizer _stringLocalizer;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfigurationParser<DropBoxesConfiguration> _configuration;

        public CBoxes(IServiceProvider serviceProvider,
            ILootBoxManager lootBoxManager,
            IStringLocalizer stringLocalizer,
            IConfigurationParser<DropBoxesConfiguration> configuration) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _lootBoxManager = lootBoxManager;
            _stringLocalizer = stringLocalizer;
            _configuration = configuration;
        }

        protected override async UniTask OnExecuteAsync()
        {
            var user = (UnturnedUser) Context.Actor;

            var lootBoxes =
                (await _lootBoxManager.GetLootBoxes(user)).Select(x => new
                {
                    Instance = x,
                    Asset = x.GetAsset(_serviceProvider)
                }).Where(x => x.Asset != null && x.Instance.Amount > 0)
                .Select(x => new
                {
                    x.Asset!.BoxId,
                    x.Asset!.Name,
                    x.Instance.Amount
                }).ToList();

            await this.PrintMessageWithIconAsync(
                _stringLocalizer["Commands:Success:LootBoxes", new {LootBoxes = lootBoxes}],
                _configuration.Instance.IconUrl);
        }
    }
}
