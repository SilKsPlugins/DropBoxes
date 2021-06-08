using MoreLinq;
using OpenMod.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DropBoxes.Configuration
{
    [Serializable]
    public class DropBoxesConfiguration
    {
        public List<LootBoxAsset> LootBoxes { get; set; } = new();

        public LootBoxAsset? GetLootBoxAssetById(string id)
        {
            return LootBoxes.FirstOrDefault(x => x.BoxId.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public LootBoxAsset? GetLootBoxAssetByName(string name)
        {
            name = name.ToLower();

            return LootBoxes.Where(x => x.Name.ToLower().Contains(name))
                .MinBy(x => StringHelper.LevenshteinDistance(x.Name.ToLower(), name))
                .FirstOrDefault();
        }
    }
}
