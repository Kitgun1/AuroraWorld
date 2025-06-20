using System;
using System.Collections.Generic;

namespace AuroraWorld.Gameplay.World
{
    [Serializable]
    public class WorldState
    {
        public List<Hexagon> Hexagons = new();
    }
}