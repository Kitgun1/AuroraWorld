using System;

namespace AuroraWorld.Gameplay.World.Terrain
{
    [Serializable]
    public class GeographyInfo
    {
        public int Elevation;
        public float Temperature;
        public float Humidity;
        public TerrainBiome TerrainBiome;

        public GeographyInfo(int elevation, float temperature, float humidity, TerrainBiome terrainBiome)
        {
            Elevation = elevation;
            Temperature = temperature;
            Humidity = humidity;
            TerrainBiome = terrainBiome;
        }
    }
}