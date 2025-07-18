using System;
using System.Collections.Generic;
using AuroraWorld.Gameplay.World.Terrain;

namespace AuroraWorld.Gameplay.World
{
    [Serializable]
    public class TerrainState
    {
        /// <summary> Loaded Hexagons </summary>
        public List<Hexagon> Hexagons = new();
    }
}