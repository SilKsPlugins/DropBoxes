using DropBoxes.API;
using DropBoxes.Database;
using DropBoxes.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

        public LootBoxManager(DropBoxesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ILootBoxInstance>> GetLootBoxes(UnturnedUser user)
        {
            return await _dbContext.LootBoxes.Where(x => x.SteamId == user.SteamId.m_SteamID).ToListAsync();
        }
        
        public async Task GiveLootBox(ulong steamId, ILootBoxAsset lootBoxAsset)
        {
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

                        var databaseValues = await entry.GetDatabaseValuesAsync();

                        var amount = databaseValues.GetValue<uint>(nameof(LootBoxInstance.Amount));

                        entry.OriginalValues.SetValues(new {Amount = amount + 1});
                    }
                }
            }
        }

        public async Task<bool> RemoveLootBox(ulong steamId, ILootBoxAsset lootBoxAsset)
        {
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
                    return true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        if (entry.Entity is not LootBoxInstance)
                            throw new NotSupportedException("Do not know how to handle concurrency conflicts for " +
                                                            entry.Metadata.Name);

                        var databaseValues = await entry.GetDatabaseValuesAsync();

                        var amount = databaseValues.GetValue<uint>(nameof(LootBoxInstance.Amount));

                        if (amount <= 0) return false;

                        entry.OriginalValues.SetValues(new { Amount = amount - 1 });
                    }
                }
            }
        }
    }
}
