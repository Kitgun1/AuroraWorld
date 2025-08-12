using System;
using UnityEngine;

namespace AuroraWorld.Gameplay.World.Entity
{
    [Serializable]
    public abstract class WorldEntity
    {
        public int MaxHealth;
        public int Health;
        public Vector3Int[] ControlledCubePositions;
        public WorldEntity[] DestroyedState;
        public Type EntityType;

        protected WorldEntity(int initialHealth, Vector3Int[] controlledCubePositions, 
            params WorldEntity[] destroyedState)
        {
            MaxHealth = initialHealth;
            Health = initialHealth;
            ControlledCubePositions = controlledCubePositions;
            DestroyedState = destroyedState;
        }

        protected WorldEntity(int maxHealth, int health, Vector3Int[] controlledCubePositions, 
            params WorldEntity[] destroyedState)
        {
            MaxHealth = maxHealth;
            Health = health;
            ControlledCubePositions = controlledCubePositions;
            DestroyedState = destroyedState;
        }
    }
}