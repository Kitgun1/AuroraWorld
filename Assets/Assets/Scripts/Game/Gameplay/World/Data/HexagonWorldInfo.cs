using System;

namespace AuroraWorld.Gameplay.World
{
    [Serializable]
    public class HexagonWorldInfo
    {
        public bool IsLand;
        public int Elevation;
        public float Temperature;
        public float Humidity;
        public FogOfWarState FogOfWarState;
    }
}