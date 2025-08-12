using System;

namespace AuroraWorld.Gameplay.World.Terrain
{
    [Serializable]
    public class GeographyInfo
    {
        public int Elevation;
        public float Temperature;
        public float Humidity;
        public float Vegetation;
        public TerrainBiome TerrainBiome;

        public GeographyInfo(int elevation, float temperature, float vegetation, float humidity, TerrainBiome terrainBiome)
        {
            Elevation = elevation;
            Temperature = temperature;
            Vegetation = vegetation;
            TerrainBiome = terrainBiome;
        }
    }
}