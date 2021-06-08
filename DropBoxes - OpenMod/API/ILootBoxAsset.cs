using System.Collections.Generic;

namespace DropBoxes.API
{
    public interface ILootBoxAsset
    {
        string BoxId { get; }

        string Name { get; }

        string? Permission { get; }

        ILootBoxReward SelectReward();

        IReadOnlyCollection<ILootBoxReward> GetRewards();
    }
}
