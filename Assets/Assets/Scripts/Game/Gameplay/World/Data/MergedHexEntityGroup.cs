using System;
using System.Collections.Generic;

namespace AuroraWorld.Gameplay.World.Geometry
{
    [Serializable]
    public class MergedHexEntityGroup
    {
        public List<HexEntity> HexagonEntities = new();
    }
}