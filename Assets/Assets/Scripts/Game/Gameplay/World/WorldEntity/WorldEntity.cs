using System;
using UnityEngine;

namespace AuroraWorld.Gameplay.World
{
    [Serializable]
    public abstract class WorldEntity
    {
        public string Name;
        public Vector3Int[] Positions;
        public int MaxHealth;
        public int Health;
        // Ресурсы, которые будут получены после добычи пешкой
        public WorldEntity NextState;
        
        public WorldEntity EntityType { get; protected set; }

        protected WorldEntity(string name, Vector3Int[] positions, int initialHealth, WorldEntity nextState = null)
        {
            Name = name;
            Positions = positions;
            MaxHealth = initialHealth;
            Health = initialHealth;
            NextState = nextState;
        }
    }
}