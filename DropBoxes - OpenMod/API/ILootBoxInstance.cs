using System;

namespace DropBoxes.API
{
    public interface ILootBoxInstance
    {
        ulong SteamId { get; }

        string LootBoxId { get; }

        uint Amount { get; }

        ILootBoxAsset? GetAsset(IServiceProvider serviceProvider);
    }
}
