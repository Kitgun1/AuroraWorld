using System;
using System.Collections.Generic;
using AuroraWorld.Gameplay.World.Entity;

namespace AuroraWorld.Gameplay.World
{
    [Serializable]
    public class EntityState
    {
        /// <summary> Loaded Entities </summary>
        public List<WorldEntity> Entities = new();
    }
}