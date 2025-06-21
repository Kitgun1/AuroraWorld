using System;
using System.Collections.Generic;

namespace AuroraWorld.Gameplay.World
{
    [Serializable]
    public class WorldEntityState
    {
        public List<WorldEntity> Entities = new();
    }
}