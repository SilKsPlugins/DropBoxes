using DropBoxes.API;
using DropBoxes.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SilK.Unturned.Extras.Configuration;
using System;
using System.ComponentModel.DataAnnotations;

namespace DropBoxes.Database.Models
{
    public class LootBoxInstance : ILootBoxInstance
    {
        [Key]
        public ulong SteamId { get; set; }

        [Key]
        public string LootBoxId { get; set; } = "";

        [ConcurrencyCheck]
        public uint Amount { get; set; } = 0;

        public ILootBoxAsset? GetAsset(IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetRequiredService<IConfigurationParser<DropBoxesConfiguration>>();

            return configuration.Instance.GetLootBoxAssetById(LootBoxId);
        }
    }
}
