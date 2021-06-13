using Cysharp.Threading.Tasks;
using DropBoxes.API;
using DropBoxes.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Ioc;
using OpenMod.API.Permissions;
using OpenMod.API.Prioritization;
using OpenMod.Unturned.Players.Stats.Events;
using OpenMod.Unturned.Users;
using SDG.Unturned;
using SilK.Unturned.Extras.Configuration;
using SilK.Unturned.Extras.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DropBoxes.LootBoxes
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton, Priority = Priority.Lowest)]
    public class LootBoxAwarder : ILootBoxAwarder,
        IInstanceAsyncEventListener<UnturnedPlayerStatIncrementedEvent>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IOpenModComponent _openModComponent;
        private readonly IPermissionRegistry _permissionRegistry;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IUnturnedUserDirectory _userDirectory;
        private readonly IConfigurationParser<DropBoxesConfiguration> _configuration;
        private readonly IStringLocalizer _stringLocalizer;
        private readonly ILogger<LootBoxAwarder> _logger;

        private readonly Random _rng;

        public LootBoxAwarder(IServiceProvider serviceProvider,
            IOpenModComponent openModComponent,
            IPermissionRegistry permissionRegistry,
            IPermissionChecker permissionChecker,
            IUnturnedUserDirectory userDirectory,
            IConfigurationParser<DropBoxesConfiguration> configuration,
            IStringLocalizer stringLocalizer,
            ILogger<LootBoxAwarder> logger)
        {
            _serviceProvider = serviceProvider;
            _openModComponent = openModComponent;
            _permissionRegistry = permissionRegistry;
            _permissionChecker = permissionChecker;
            _userDirectory = userDirectory;
            _configuration = configuration;
            _stringLocalizer = stringLocalizer;
            _logger = logger;

            _rng = new Random();
        }

        private ILootBoxManager GetLootBoxManager()
        {
            return _serviceProvider.GetRequiredService<ILootBoxManager>();
        }

        private async Task<ICollection<LootBoxAsset>> GetLootBoxesToGive(UnturnedUser user, EPlayerStat stat)
        {
            var config = _configuration.Instance;

            var lootBoxes = new List<LootBoxAsset>();

            foreach (var lootBox in config.LootBoxes)
            {
                var chance = stat switch
                {
                    EPlayerStat.KILLS_PLAYERS => lootBox.Chances.PlayerKill,
                    EPlayerStat.KILLS_ZOMBIES_NORMAL => lootBox.Chances.ZombieKill,
                    EPlayerStat.KILLS_ZOMBIES_MEGA => lootBox.Chances.MegaZombieKill,
                    EPlayerStat.KILLS_ANIMALS => lootBox.Chances.AnimalKill,
                    EPlayerStat.FOUND_PLANTS => lootBox.Chances.HarvestPlant,
                    EPlayerStat.FOUND_RESOURCES => lootBox.Chances.HarvestResource,
                    _ => 0
                };

                if (chance == 0)
                {
                    continue;
                }

                var random = _rng.NextDouble() * 100;

                if (random >= chance)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(lootBox.Permission))
                {
                    if (_permissionRegistry.FindPermission(_openModComponent, lootBox.Permission!) == null)
                    {
                        _permissionRegistry.RegisterPermission(_openModComponent, lootBox.Permission!,
                            "Grants access to naturally obtain a loot box.", PermissionGrantResult.Deny);
                    }

                    if (await _permissionChecker.CheckPermissionAsync(user, lootBox.Permission!) !=
                        PermissionGrantResult.Grant)
                    {
                        continue;
                    }
                }

                lootBoxes.Add(lootBox);
            }

            return lootBoxes;
        }

        public async UniTask HandleEventAsync(object? sender, UnturnedPlayerStatIncrementedEvent @event)
        {
            await UniTask.SwitchToThreadPool();

            var user = _userDirectory.GetUser(@event.Player.Player);

            var lootBoxes = await GetLootBoxesToGive(user, @event.Stat);

            if (lootBoxes.Count == 0) return;

            var lootBoxManager = GetLootBoxManager();

            foreach (var lootBox in lootBoxes)
            {
                _logger.LogDebug($"Giving player {user.DisplayName} ({user.SteamId}) loot box {lootBox.BoxId}.");

                await lootBoxManager.GiveLootBox(@event.Player.SteamId.m_SteamID, lootBox);

                await @event.Player.PrintMessageAsync(_stringLocalizer["Natural:FoundLootBox",
                    new {LootBox = lootBox}]);
            }
        }
    }
}
