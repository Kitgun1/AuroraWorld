using System;
using AuroraWorld.Gameplay.World.Data;
using UnityEngine;

namespace AuroraWorld.Gameplay.World.Geometry
{
    [Serializable]
    public class Hexagon
    {
        public Vector3Int Position;
        public HexagonWorldInfo WorldInfo;

        public Hexagon(Vector3Int position)
        {
            Position = position;
        }
    }
}