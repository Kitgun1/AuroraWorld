using AuroraWorld.Gameplay.World.Geometry;
using UnityEngine;

namespace AuroraWorld.Gameplay.World.Entity
{
    public static class WorldEntityFactory
    {
        #region Rocks

        public static RockEntity InstanceSmallRockEntity(Vector3Int center, int initialHealth = 100)
        {
            var rock = new RockEntity(initialHealth, new[] { center });
            return rock;
        }

        public static RockEntity InstanceRockEntity(Vector3Int center, int initialHealth = 300)
        {
            var controlledPositions = new Vector3Int[]
            {
                center,
                center + DirectionType.Right.ToCubeDirection(),
                center + DirectionType.TopRight.ToCubeDirection()
            };
            var smallRocks = new WorldEntity[]
            {
                InstanceSmallRockEntity(center, initialHealth / 6),
                InstanceSmallRockEntity(center + DirectionType.Right.ToCubeDirection(), initialHealth / 6),
                InstanceSmallRockEntity(center + DirectionType.TopRight.ToCubeDirection(), initialHealth / 6),
            };
            var rock = new RockEntity(initialHealth, controlledPositions, smallRocks);
            return rock;
        }

        public static RockEntity InstanceBigRockEntity(Vector3Int center, int initialHealth = 900)
        {
            var controlledPositions = CubeMath.Range(center, 1);
            DirectionType randFirstRockDirection = (DirectionType)Random.Range(1, 5);
            DirectionType randSecondRockDirection;
            while (true)
            {
                randSecondRockDirection = (DirectionType)Random.Range(1, 5);
                if (randFirstRockDirection != randSecondRockDirection) break;
            }

            var rocks = new WorldEntity[]
            {
                InstanceRockEntity(center, initialHealth / 6),
                InstanceSmallRockEntity(center + randFirstRockDirection.ToCubeDirection(), initialHealth / 12),
                InstanceSmallRockEntity(center + randSecondRockDirection.ToCubeDirection(), initialHealth / 12),
            };
            var bigRock = new RockEntity(initialHealth, controlledPositions, rocks);
            return bigRock;
        }

        #endregion
    }
}