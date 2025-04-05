using System;
using UnityEngine;

namespace Assets.Scripts.Game.Gameplay.World
{
    [Serializable]
    public class HexagonEntity
    {
        public Vector2Int Position;
        public BiomeType BiomeType;

        public HexagonEntity() { }

        public HexagonEntity(Vector2Int position, BiomeType biomeType)
        {
            Position = position;
            BiomeType = biomeType;
        }
    }
}