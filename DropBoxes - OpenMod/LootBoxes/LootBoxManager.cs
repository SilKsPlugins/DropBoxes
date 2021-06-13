using DropBoxes.API;
using DropBoxes.Database;
using DropBoxes.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenMod.API.Ioc;
using OpenMod.API.Prioritization;
using OpenMod.Unturned.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DropBoxes.LootBoxes
{
    [PluginServiceImplementation(Lifetime = ServiceLifetime.Transient, Priority = Priority.Lowest)]
    public class LootBoxManager : ILootBoxManager
    {
        private readonly DropBoxesDbContext _dbContext;
        private readonly ILogger<LootBoxManager> _logger;

        public LootBoxManager(DropBoxesDbContext dbContext,
            ILogger<LootBoxManager> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IEnumerable<ILootBoxInstance>> GetLootBoxes(UnturnedUser user)
        {
            return await _dbContext.LootBoxes.Where(x => x.SteamId == user.SteamId.m_SteamID).ToListAsync();
        }
        
        public async Task GiveLootBox(ulong steamId, ILootBoxAsset lootBoxAsset)
        {
            _logger.LogDebug($"Giving {steamId} a {lootBoxAsset.BoxId} loot box.");
            var instance = await _dbContext.LootBoxes.FindAsync(steamId, lootBoxAsset.BoxId);

            if (instance == null)
            {
                instance = new LootBoxInstance
                {
                    SteamId = steamId,
                    LootBoxId = lootBoxAsset.BoxId,
                    Amount = 1
                };

                await _dbContext.LootBoxes.AddAsync(instance);
            }
            else
            {
                instance.Amount++;
                _dbContext.LootBoxes.Update(instance);
            }

            var saved = false;

            while (!saved)
            {
                try
                {
                    await _dbContext.SaveChangesAsync();
                    saved = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        if (entry.Entity is not LootBoxInstance)
                            throw new NotSupportedException("Do not know how to handle concurrency conflicts for " +
                                                            entry.Metadata.Name);

                        var proposedValues = entry.CurrentValues;
                        var databaseValues = await entry.GetDatabaseValuesAsync();

                        var amount = databaseValues.GetValue<uint>(nameof(LootBoxInstance.Amount));

                        proposedValues.SetValues(new { Amount = amount + 1 });

                        // Bypass next concurrency check
                        entry.OriginalValues.SetValues(databaseValues);
                    }
                }
            }
        }

        public async Task<bool> RemoveLootBox(ulong steamId, ILootBoxAsset lootBoxAsset)
        {
            _logger.LogDebug($"Removing a {lootBoxAsset.BoxId} loot box from {steamId}.");

            var instance = await _dbContext.LootBoxes.FindAsync(steamId, lootBoxAsset.BoxId);

            if (instance is not {Amount: > 0})
            {
                return false;
            }

            instance.Amount--;
            _dbContext.LootBoxes.Update(instance);

            while (true)
            {
                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogDebug($"Removed a {lootBoxAsset.BoxId} loot box from {steamId}.");
                    return true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        if (entry.Entity is not LootBoxInstance)
                            throw new NotSupportedException("Do not know how to handle concurrency conflicts for " +
                                                            entry.Metadata.Name);

                        var proposedValues = entry.CurrentValues;
                        var databaseValues = await entry.GetDatabaseValuesAsync();

                        var amount = databaseValues.GetValue<uint>(nameof(LootBoxInstance.Amount));

                        if (amount <= 0) return false;

                        proposedValues.SetValues(new { Amount = amount - 1 });

                        // Bypass next concurrency check
                        entry.OriginalValues.SetValues(databaseValues);
                    }
                }
            }
        }
    }
}
