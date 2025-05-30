using System;
using System.Collections.Generic;
using AuroraWorld.Gameplay.World.Data;
using AuroraWorld.Gameplay.World.Geometry;
using AuroraWorld.Gameplay.World.Root;
using AuroraWorld.Utils;
using UnityEngine;
using Random = System.Random;

namespace AuroraWorld.Gameplay.World
{
    public static class Geography
    {
        private static int _seedHash;
        private static float _continentSeed;
        private static float _rivesSeed;
        private static float _colorSeed;
        private static readonly Dictionary<BiomeType, float> BiomeSeed = new();

        private static bool _seedsInitialized = false;

        public static void SetSeed(string seed)
        {
            _seedHash = seed.GetHashCode();
            var random = new Random(_seedHash);
            var details = 100_000f;

            _continentSeed = random.Next(-1 * (int)details, 1 * (int)details) / (details / 100);
            _rivesSeed = random.Next(-2 * (int)details, 2 * (int)details) / (details / 100);
            _colorSeed = random.Next(-3 * (int)details, 3 * (int)details) / (details / 100);
            // _waterSeed = random.Next(-1 * (int)details, 1 * (int)details) / (details / 100);
            // _mountainsSeed = random.Next(-2 * (int)details, 2 * (int)details) / (details / 100);
            // _humiditySeed = random.Next(-4 * (int)details, 4 * (int)details) / (details / 100);
            // _temperatureSeed = random.Next(-5 * (int)details, 5 * (int)details) / (details / 100);

            for (var i = 0; i < Enum.GetNames(typeof(BiomeType)).Length; i++)
            {
                var biomeName = Enum.GetNames(typeof(BiomeType))[i];
                var randomSeed = random.Next((-10 - i) * (int)details, (10 + i) * (int)details) / (details / 100);
                BiomeSeed[Enum.Parse<BiomeType>(biomeName, true)] = randomSeed;
            }

            _seedsInitialized = true;
        }

        public static float RivesElevation(this GeoConfiguration configuration, Vector2Int hex)
        {
            if (!_seedsInitialized) throw new Exception("Seed not initialized. Use SetSeed for set seed!");

            var e = FBM(hex.x, hex.y, _rivesSeed, configuration.RivesScale, 10);
            e = Mathf.Pow(Mathf.Clamp01((e - 0.3f) / (0.7f - 0.3f)), 5);

            return e;
        }

        public static float GetElevation(this GeoConfiguration configuration, Vector2Int hex)
        {
            if (!_seedsInitialized) throw new Exception("Seed not initialized. Use SetSeed for set seed!");

            var e = FBM(hex.x, hex.y, _continentSeed, configuration.ContinentScale, 10);
            e = e * e * e;

            return Mathf.Clamp01(Mathf.Floor(e * 50) / 49);
        }

        public static float GetTemperature(this GeoConfiguration configuration, Vector2Int hex)
        {
            if (!_seedsInitialized) throw new Exception("Seed not initialized. Use SetSeed for set seed!");

            return Mathf.Clamp01(Mathf.Floor(1 * 50) / 49);
        }

        public static float GetHumidity(this GeoConfiguration configuration, Vector2Int hex)
        {
            if (!_seedsInitialized) throw new Exception("Seed not initialized. Use SetSeed for set seed!");

            return Mathf.Clamp01(Mathf.Floor(1 * 50) / 49);
        }

        public static Color32 GetColor(this GeoConfiguration configuration, Vector2Int hex)
        {
            var height = configuration.GetElevation(hex);
            Color32 color;
            var gamma = (int)Mathf.Lerp(-40, 40, FBM(hex.x, hex.y, _colorSeed, 2, 1));
            if (height >= configuration.LandMinElevation)
                color = new Color32(0, 155, 0, 255).AddGamma(gamma);
            else color = new Color32(28, 169, 201, 255).AddGamma(gamma);

            return color;
        }

        public static HexWorldInfoProxy GetHexagonInfo(this GeoConfiguration configuration, Vector2Int hex)
        {
            var hexagonInfo = new HexWorldInfo();
            var hexagonInfoProxy = new HexWorldInfoProxy(hexagonInfo);
            var elevation = Mathf.Clamp(configuration.GetElevation(hex), 0, configuration.LandMinElevation);
            var rivesElevation = configuration.RivesElevation(hex);
            hexagonInfoProxy.IsLand.Value = configuration.LandMinElevation >= elevation ||
                                            configuration.RivesMinElevation >= rivesElevation;
            hexagonInfoProxy.Elevation.Value = elevation;
            hexagonInfoProxy.Temperature.Value = configuration.GetTemperature(hex);
            hexagonInfoProxy.Humidity.Value = configuration.GetHumidity(hex);

            return hexagonInfoProxy;
        }

        /// <summary> FractionalBrownianMotion </summary>
        private static float FBM(int x, int y, float seed, float scale, int octaves = 5)
        {
            var total = 0f;
            var frequency = 1f;
            var amplitude = 0f;
            for (var i = 0; i < octaves; i++)
            {
                total += frequency * Noise(new Vector2(x, y) * (1 / frequency));
                amplitude += frequency;
                frequency /= 2;
            }

            total /= amplitude;
            return total;

            float Noise(Vector2 pos) => Mathf.Clamp01(
                Mathf.PerlinNoise(seed + scale * pos.x, seed + scale * pos.y)
            );
        }

        private static float ExpCurve(float value, float k) => (Mathf.Exp(k * value) - 1) / (Mathf.Exp(k) - 1);
    }
}