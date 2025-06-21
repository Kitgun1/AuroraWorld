using System;
using UnityEngine;

namespace AuroraWorld.Gameplay.World
{
    [Serializable]
    public class RockEntity : WorldEntity
    {
        public RockEntity(string name, int initialHealth, Vector3Int[] positions, WorldEntity nextState = null) 
            : base(name, positions, initialHealth, nextState)
        {
            EntityType = this;
        }
    }
}