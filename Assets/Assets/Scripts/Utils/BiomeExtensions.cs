using System;
using AuroraWorld.Gameplay.World.Root;
using UnityEngine;

namespace Assets.Utils.Extensions
{
    public static class BiomeExtensions
    {
        public static Color ToColor(this BiomeType biomeType)
        {
            return biomeType switch
            {
                BiomeType.None => Color.black,

                BiomeType.Plain => new Color(0.53f, 0.75f, 0.44f),
                BiomeType.Swamp => new Color(0.36f, 0.43f, 0.23f),
                BiomeType.Desert => new Color(0.89f, 0.76f, 0.48f),
                BiomeType.RockyWasteland => new Color(0.66f, 0.66f, 0.66f),
                BiomeType.Mountains => new Color(0.65f, 0.65f, 0.65f),
                BiomeType.Highland => new Color(0.69f, 0.72f, 0.64f),
                BiomeType.SnowyPlain => new Color(0.94f, 0.97f, 1f),
                BiomeType.ColdOcean => new Color(0.23f, 0.37f, 0.55f),
                BiomeType.WarmOcean => new Color(0.11f, 0.66f, 0.79f),
                BiomeType.Ocean => new Color(0.27f, 0.51f, 0.71f),
                _ => throw new ArgumentOutOfRangeException(nameof(biomeType), biomeType, null)
            };
        }

        public static BiomeType ToBiome(this Color color)
        {
            if (color.Equals(Color.black))
                return BiomeType.None;
            if (color.Equals(Color.magenta))
                return BiomeType.None;
            if (color.Equals(Color.cyan))
                return BiomeType.None;
            if (color.Equals(new Color(0.53f, 0.75f, 0.44f)))
                return BiomeType.Plain;
            if (color.Equals(new Color(0.36f, 0.43f, 0.23f)))
                return BiomeType.Swamp;
            if (color.Equals(new Color(0.89f, 0.76f, 0.48f)))
                return BiomeType.Desert;
            if (color.Equals(new Color(0.66f, 0.66f, 0.66f)))
                return BiomeType.RockyWasteland;
            if (color.Equals(new Color(0.65f, 0.65f, 0.65f)))
                return BiomeType.Mountains;
            if (color.Equals(new Color(0.69f, 0.72f, 0.64f)))
                return BiomeType.Highland;
            if (color.Equals(new Color(0.94f, 0.97f, 1f)))
                return BiomeType.SnowyPlain;
            if (color.Equals(new Color(0.23f, 0.37f, 0.55f)))
                return BiomeType.ColdOcean;
            if (color.Equals(new Color(0.11f, 0.66f, 0.79f)))
                return BiomeType.WarmOcean;
            if (color.Equals(new Color(0.27f, 0.51f, 0.71f)))
                return BiomeType.Ocean;
            throw new ArgumentOutOfRangeException(nameof(color), color, null);
        }
    }
}