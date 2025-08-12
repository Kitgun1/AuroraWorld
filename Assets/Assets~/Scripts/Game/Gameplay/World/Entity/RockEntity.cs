using System;
using UnityEngine;

namespace AuroraWorld.Gameplay.World.Entity
{
    public class RockEntity : WorldEntity, ICloneable
    {
        public RockEntity(int initialHealth, Vector3Int[] controlledCubePositions, params WorldEntity[] destroyedState)
            : base(initialHealth, controlledCubePositions, destroyedState)
        {
            EntityType = typeof(RockEntity);
        }

        public RockEntity(int maxHealth, int health, Vector3Int[] controlledCubePositions, params WorldEntity[] destroyedState)
            : base(maxHealth, health, controlledCubePositions, destroyedState)
        {
            EntityType = typeof(RockEntity);
        }

        public object Clone()
        {
           return new RockEntity(MaxHealth, Health, ControlledCubePositions, DestroyedState);
        }
    }
}