using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.Game.Gameplay.World
{
    public static class WorldStateExtensions
    {
        private static float _biomeSeed;
        private static float _humiditySeed;
        private static float _temperatureSeed;
        private static readonly Dictionary<BiomeType, float> _biomeSeeds = new();

        public static void UpdateRandomSeed()
        {
            var random = new Random(Guid.NewGuid().GetHashCode());
            _biomeSeed = random.Next(0, int.MaxValue) / 9999f;
            _humiditySeed = random.Next(2, int.MaxValue) / 9999f;
            _temperatureSeed = random.Next(3, int.MaxValue) / 9999f;
            var biomeValues = Enum.GetValues(typeof(BiomeType)).OfType<BiomeType>().ToArray();
            for (int i = 0; i < biomeValues.Length; i++)
            {
                _biomeSeeds.TryAdd(biomeValues[i], random.Next(3 + i, int.MaxValue) / 9999f);
            }
        }

        public static BiomeType GetBiomeBySettings(this GenerationSettings generationSettings, Vector2Int position)
        {
            // global elevation
            var globalScale = generationSettings.ContinentScale;
            var ge = 1 * Noise(_biomeSeed, globalScale, new Vector2(1 * position.x, 1 * position.y))
                     + 0.3f * Noise(_biomeSeed, globalScale, new Vector2(2 * position.x, 2 * position.y))
                     + 0.1f * Noise(_biomeSeed, globalScale, new Vector2(4 * position.x, 4 * position.y));
            ge /= 1 + 0.3f + 0.1f;
            ge = Mathf.Clamp01(Mathf.Pow(ge, (float)Math.Exp(generationSettings.Exponent)));

            // Основные биобы (вода, берег, пляж, ..)
            var globalBiome = generationSettings.GlobalBiomes.FirstOrDefault(e => e.MinHeight <= ge);

            if (globalBiome.LocalBiomes is null or { Count: 0 }) return globalBiome.Biome;
            var lt = Noise(_temperatureSeed, globalBiome.TemperatureScale, position); // local temperature
            var lh = Noise(_humiditySeed, globalBiome.HumidityScale, position); // local humidity

            // filtered biomes by [temperature, humidity, local elevation]
            var localBiomes = globalBiome.LocalBiomes.Where(settings =>
                RangeContains(lt, settings.FilterTemperatureRange) &&
                RangeContains(lh, settings.FilterHumidityRange)
            ).ToList();

            // throw warning if filteredBiomes is empty
            if (localBiomes.Count == 0)
            {
                return globalBiome.Biome;
            }

            // multi-layered local biome generate
            for (int i = localBiomes.Count - 1; i >= 0; i--)
            {
                var localBiome = localBiomes[i];
                var biome = localBiome.BiomeType;
                var seed = _biomeSeeds[biome];
                var scale = localBiome.Scale;
                var height = Noise(seed, scale, position);
                if (localBiome.MinHeight >= height) return biome;
            }

            return globalBiome.Biome;

            float Noise(float seed, float scale, Vector2 pos) => Mathf.Clamp01(
                Mathf.PerlinNoise(seed + scale * pos.x, seed + scale * pos.y)
            );

            bool RangeContains(float value, Vector2 range) => range.x <= value && value <= range.y;

            void LogWarning() => Debug.LogWarning(
                $"Not found biome by [\n  globalBiome: {globalBiome.Biome}\n  temperature: {lt}\n  humidity: {lh}\n]"
            );
        }

        public static Color ToColor(this BiomeType biomeType)
        {
            return biomeType switch
            {
                BiomeType.Default => Color.black,

                BiomeType.ColdOcean => new Color(0f, 0f, 0.69f),
                BiomeType.Ocean => Color.blue,
                BiomeType.WarmOcean => new Color(0.16f, 0.73f, 1f),

                BiomeType.Beach => Color.yellow,

                BiomeType.Lawn => Color.green,
                BiomeType.Forest => new Color(0f, 0.75f, 0f),
                BiomeType.Desert => new Color(0.84f, 0.67f, 0.18f),
                BiomeType.Swamp => new Color(0.38f, 0.53f, 0.16f),
                BiomeType.Jungle => new Color(0.16f, 0.62f, 0f),
                BiomeType.FlowerForest => new Color(1f, 0.5f, 0.53f),
                BiomeType.Savanna => new Color(0.61f, 1f, 0.15f),
                BiomeType.Volcanic => new Color(0.25f, 0.14f, 0.12f),
                BiomeType.MushroomsFields => new Color(0.55f, 0.25f, 0.54f),
                BiomeType.Tundra => new Color(0.56f, 1f, 0.38f),
                BiomeType.SnowLawn => new Color(0.79f, 0.79f, 0.79f),

                BiomeType.Hill => Color.gray,
                BiomeType.Mountain => Color.white,
                BiomeType.SnowyPeaks => new Color(0.69f, 0.82f, 1f),
                _ => throw new ArgumentOutOfRangeException(nameof(biomeType), biomeType, null)
            };
        }
    }
}