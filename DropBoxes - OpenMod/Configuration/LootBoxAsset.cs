using DropBoxes.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DropBoxes.Configuration
{
    [Serializable]
    public class LootBoxAsset : ILootBoxAsset
    {
        private static readonly Random Rng = new();

        public string BoxId { get; set; } = "";

        public string Name { get; set; } = "";

        public string? Permission { get; set; } = null;

        public Chances Chances { get; set; } = new();

        public List<LootBoxReward> Rewards { get; set; } = new();

        public IReadOnlyCollection<ILootBoxReward> GetRewards() => Rewards.AsReadOnly();

        public ILootBoxReward SelectReward()
        {
            if (Rewards.Count == 0)
                throw new InvalidOperationException("No configured rewards for loot box");

            var rewards = Rewards.ToList();

            var weights = new double[rewards.Count];

            double sum = 0;

            for (var i = 0; i < weights.Length; i++)
            {
                sum += rewards[i].Weight;
                weights[i] = sum;
            }

            var rng = Rng.NextDouble() * sum;

            for (var i = 0; i < weights.Length; i++)
            {
                if (weights[i] < rng)
                {
                    return rewards[i];
                }
            }

            return rewards.Last();
        }
    }
}
