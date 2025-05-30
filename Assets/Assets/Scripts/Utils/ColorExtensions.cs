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
    }
}