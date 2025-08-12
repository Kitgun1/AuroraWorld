using System;
using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.World.Entity.Proxy
{
    public abstract class WorldEntityProxy
    {
        public readonly ReactiveProperty<int> MaxHealth;
        public readonly ReactiveProperty<int> Health;
        public readonly Vector3Int[] ControlledCubePositions;
        public readonly WorldEntityProxy[] DestroyedState;
        public readonly Type EntityType;

        public readonly WorldEntity Origin;

        public WorldEntityProxy(WorldEntity origin)
        {
            Origin = origin;

            MaxHealth = new ReactiveProperty<int>(Origin.MaxHealth);
            Health = new ReactiveProperty<int>(Origin.Health);
            ControlledCubePositions = Origin.ControlledCubePositions;
            EntityType = Origin.EntityType;

            DestroyedState = new WorldEntityProxy[Origin.DestroyedState.Length];
            for (var i = 0; i < Origin.DestroyedState.Length; i++)
            {
                var entity = Origin.DestroyedState[i];
                DestroyedState[i] = InstanceProxy(entity);
            }
        }

        public static WorldEntityProxy InstanceProxy(WorldEntity origin)
        {
            return origin switch
            {
                RockEntity => new RockEntityProxy(origin),
                _ => throw new ArgumentOutOfRangeException(nameof(origin), origin, null)
            };
        }
    }
}