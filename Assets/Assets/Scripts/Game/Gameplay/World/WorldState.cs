using System;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Gameplay.World
{
    [Serializable]
    public class WorldState
    {
        public List<HexagonEntity> Hexagons = new();
    }
}