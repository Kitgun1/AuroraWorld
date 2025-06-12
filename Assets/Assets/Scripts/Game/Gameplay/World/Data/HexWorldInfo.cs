using System;

namespace AuroraWorld.Gameplay.World.Data
{
    [Serializable]
    public class HexWorldInfo
    {
        public bool IsLand;
        public float Elevation;
        public float Temperature;
        public float Humidity;
        public FogOfWarHexState FogOfWarState;
    }
}