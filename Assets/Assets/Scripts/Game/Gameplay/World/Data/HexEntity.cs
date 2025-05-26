using System;
using AuroraWorld.Gameplay.World.Data;
using UnityEngine;

namespace AuroraWorld.Gameplay.World.Geometry
{
    [Serializable]
    public class HexEntity
    {
        public Vector3Int Position;
        public HexWorldInfo WorldInfo;

        public HexEntity(Vector3Int position)
        {
            Position = position;
        }
    }
}