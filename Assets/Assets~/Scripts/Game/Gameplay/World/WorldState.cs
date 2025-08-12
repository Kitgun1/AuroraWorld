using System;

namespace AuroraWorld.Gameplay.World
{
    [Serializable]
    public class WorldState
    {
        public string WorldName;
        public string WorldSeed;
        public TerrainState TerrainState;
        public EntityState EntityState;
    }
}