using R3;
using UnityEngine;

namespace Assets.Scripts.Game.Gameplay.World
{
    public class HexagonEntityProxy
    {
        public Vector2Int Position { get; } 
        public BiomeType BiomeType { get; }

        public HexagonEntityProxy(HexagonEntity hexagonEntity)
        {
            Position = hexagonEntity.Position;
            BiomeType = hexagonEntity.BiomeType;
        }
    }
}