using AuroraWorld.Gameplay.World.Geometry;
using AuroraWorld.Gameplay.World.Terrain;
using UnityEngine;
using Random = System.Random;

namespace AuroraWorld.Gameplay.World
{
    public class Geography
    {
        private static float _continentSeed;
        private static float _elevationSeed;
        private static float _temperatureSeed;
        private static float _humiditySeed;
        private static float _rivesSeed;
        private static float _colorSeed;

        public Geography(string seed)
        {
            var seedHash = seed.GetHashCode();
            var random = new Random(seedHash);
            const float details = 100_000f;

            _continentSeed = random.Next(-1 * (int)details, 1 * (int)details) / (details / 100);
            _elevationSeed = random.Next(-2 * (int)details, 2 * (int)details) / (details / 100);
            _temperatureSeed = random.Next(-3 * (int)details, 3 * (int)details) / (details / 100);
            _humiditySeed = random.Next(-4 * (int)details, 4 * (int)details) / (details / 100);
            _rivesSeed = random.Next(-5 * (int)details, 5 * (int)details) / (details / 100);
            _colorSeed = random.Next(-6 * (int)details, 6 * (int)details) / (details / 100);
        }

        public GeographyInfo InstanceInfo(Vector3Int cubePosition, out bool isContinent, out bool isRives)
        {
            var axial = cubePosition.ToHex();

            var continentElevation = FBM(axial.x, axial.y, _continentSeed, 0.004f, 8);
            //continentElevation = Mathf.Pow(continentElevation, 3);
            isContinent = continentElevation >= 0.1f;

            var rivesElevation = FBM(axial.x, axial.y, _continentSeed, 0.01f, 10);
            rivesElevation = Mathf.Pow(Mathf.Clamp01((rivesElevation - 0.3f) / (0.7f - 0.3f)), 5);
            ;
            isRives = rivesElevation >= 0.85f;

            bool isLand = isContinent && !isRives;

            var elevation = FBM(axial.x, axial.y, _elevationSeed, 0.01f, 3) + Mathf.Clamp(continentElevation, 0, 0.1f);
            var temperature = FBM(axial.x, axial.y, _temperatureSeed, 0.01f, 3);
            var humidity = FBM(axial.x, axial.y, _humiditySeed, 0.01f, 3);

            var terrainBiome = isLand
                ? TerrainBiome.Plains
                : TerrainBiome.Ocean;

            var elevationInt = Mathf.RoundToInt(elevation / GeometryHexagon.ELEVATION_STEP);
            return new GeographyInfo(elevationInt, temperature, humidity, terrainBiome);
        }

        public GeographyInfo InstanceInfo(Vector3Int cubePosition) => InstanceInfo(cubePosition, out _, out _);

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
    }
}