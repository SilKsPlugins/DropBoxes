using System;

namespace DropBoxes.Configuration
{
    [Serializable]
    public class Chances
    {
        public float PlayerKill { get; set; } = 0;

        public float ZombieKill { get; set; } = 0;

        public float MegaZombieKill { get; set; } = 0;

        public float FoundPlants { get; set; } = 0;

        public float FoundResources { get; set; }
    }
}
