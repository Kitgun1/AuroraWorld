using System;
using AuroraWorld.Gameplay.World.Terrain;
using UnityEngine;

namespace AuroraWorld.Utils
{
    public static class ColorExtensions
    {
        public static Color32 AddGamma(this Color32 color, int gammaValue)
        {
            color.a = (byte)Mathf.Clamp(color.a + gammaValue, 0, 255);
            color.r = (byte)Mathf.Clamp(color.r + gammaValue, 0, 255);
            color.g = (byte)Mathf.Clamp(color.g + gammaValue, 0, 255);

            return color;
        }

        public static Color32 ToColor(this TerrainBiome biome)
        {
            return biome switch
            {
                TerrainBiome.Ocean => new Color32(45, 81, 255, 255),
                TerrainBiome.Plains => new Color32(51, 202, 29, 255),
                TerrainBiome.Plato => new Color32(60, 60, 60, 255),
                _ => throw new ArgumentOutOfRangeException(nameof(biome), biome, null)
            };
        }
    }
}