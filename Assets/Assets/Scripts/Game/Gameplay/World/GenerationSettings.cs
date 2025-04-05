using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Gameplay.World
{
    [Serializable]
    public class GenerationSettings
    {
        public float ContinentScale = 0.015f;
        public double Exponent = 0.8f;

        public List<GlobalBiomeSettings> GlobalBiomes = new()
        {
            new GlobalBiomeSettings
            {
                Biome = BiomeType.Mountain,
                MinHeight = 0.7f,
                HumidityScale = 0.2f,
                TemperatureScale = 0.2f,
                LocalBiomes = new List<LocalBiomeSettings>()
            },
            new GlobalBiomeSettings
            {
                Biome = BiomeType.Lawn,
                MinHeight = 0.23f,
                HumidityScale = 0.1f,
                TemperatureScale = 0.05f,
                LocalBiomes = new List<LocalBiomeSettings>
                { //    Biome Type               , h   , scale, temperature            , humidity (влажость)
                    new(BiomeType.FlowerForest   , 0.9f, 0.02f, new Vector2(0.7f, 0.8f), new Vector2(0.4f, 0.6f)),
                    new(BiomeType.MushroomsFields, 0.9f, 0.03f, new Vector2(0.3f, 0.5f), new Vector2(0.7f, 1.0f)),
                    new(BiomeType.Desert         , 0.8f, 0.03f, new Vector2(0.5f, 1.0f), new Vector2(0.0f, 0.4f)),
                    new(BiomeType.Tundra         , 0.8f, 0.03f, new Vector2(0.0f, 0.5f), new Vector2(0.0f, 1.0f)),
                    new(BiomeType.SnowLawn       , 0.8f, 0.01f, new Vector2(0.0f, 0.5f), new Vector2(0.0f, 1.0f)),
                    new(BiomeType.Savanna        , 0.8f, 0.04f, new Vector2(0.4f, 1.0f), new Vector2(0.5f, 1.0f)),
                    new(BiomeType.Jungle         , 0.8f, 0.04f, new Vector2(0.6f, 1.0f), new Vector2(0.7f, 1.0f)),
                    new(BiomeType.Swamp          , 0.8f, 0.04f, new Vector2(0.3f, 0.6f), new Vector2(0.7f, 1.0f)),
                    new(BiomeType.Forest         , 0.8f, 0.05f, new Vector2(0.4f, 0.7f), new Vector2(0.4f, 0.7f)),
                    new(BiomeType.Lawn           , 0.8f, 0.04f, new Vector2(0.5f, 0.7f), new Vector2(0.4f, 0.9f)),
                }
            },
            new GlobalBiomeSettings
            {
                Biome = BiomeType.Beach,
                MinHeight = 0.21f
            },
            new GlobalBiomeSettings
            {
                Biome = BiomeType.Ocean,
                MinHeight = 0.0f,
                TemperatureScale = 0.09f,
                LocalBiomes = new List<LocalBiomeSettings>
                { //Biome Type             , h,    scale, temperature             , humidity (влажость)
                    new(BiomeType.WarmOcean, 0.7f, 0.04f, new Vector2(0.55f, 1.0f), new Vector2(0.0f, 1.0f)),
                    new(BiomeType.ColdOcean, 0.7f, 0.04f, new Vector2(0.0f, 0.44f), new Vector2(0.0f, 1.0f)),
                }
            },
        };
    }

    [Serializable]
    public struct GlobalBiomeSettings
    {
        public float MinHeight;
        public BiomeType Biome; // Фундоментальные биомы (море, суша, ..)
        public float HumidityScale;
        public float TemperatureScale;

        public List<LocalBiomeSettings> LocalBiomes;
    }

    [Serializable]
    public struct LocalBiomeSettings
    {
        public BiomeType BiomeType;
        public float MinHeight;
        public float Scale;
        public Vector2 FilterTemperatureRange;
        public Vector2 FilterHumidityRange;

        public LocalBiomeSettings(BiomeType biomeType, float minHeight, float scale, Vector2 t, Vector2 h)
        {
            BiomeType = biomeType;
            MinHeight = minHeight;
            Scale = scale;
            FilterTemperatureRange = t;
            FilterHumidityRange = h;
        }
    }
}