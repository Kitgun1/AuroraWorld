using System;
using UnityEngine;

namespace AuroraWorld.Gameplay.World
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