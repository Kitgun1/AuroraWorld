using System;
using AuroraWorld.Gameplay.World.Data;
using UnityEngine;

namespace AuroraWorld.Gameplay.World
{
    [CreateAssetMenu(fileName = "new Geo Settings", menuName = "Aurora World/World/Geo Settings", order = 0)]
    public class GeoConfiguration : ScriptableObject
    {
        [Space, Header("Continent Properties")]
        [Range(0, 1)] public float ContinentScale;
        [Range(0, 100)] public int LandMinElevation;
        
        [Space, Header("Rives Properties")]
        [Range(0, 1)] public float RivesScale;
        [Range(0, 100)] public int RivesMinElevation;
        
        [Space(15), Header("Other")]
        public BiomesConfiguration BiomesConfiguration;
    }

    [Serializable]
    public struct LocalGeoSettings
    {
        public string GlobalGeoId;
        public BiomeType Biome;
        public float Scale;
        public Vector2 ContinentHeightRange;
        public Vector2 TemperatureRange;
        public Vector2 HumidityRange;

        /// <param name="globalGeoId"> Id континента </param>
        /// <param name="biome"> Тип биома </param>
        /// <param name="scale"> Масштаб биома </param>
        /// <param name="gh"> Глобальная высота </param>
        /// <param name="t"> Темпиратура </param>
        /// <param name="h"> Влажность </param>
        public LocalGeoSettings(string globalGeoId, BiomeType biome, float scale, Vector2 gh, Vector2 t, Vector2 h)
        {
            GlobalGeoId = globalGeoId;
            Biome = biome;
            Scale = scale;
            ContinentHeightRange = gh;
            TemperatureRange = t;
            HumidityRange = h;
        }
    }
}