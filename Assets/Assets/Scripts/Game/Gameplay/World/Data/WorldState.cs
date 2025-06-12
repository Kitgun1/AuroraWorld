using System;
using System.Collections.Generic;

namespace AuroraWorld.Gameplay.World.Geometry
{
    [Serializable]
    public class WorldState
    {
        public List<HexEntity> Hexagons = new();
    }
}