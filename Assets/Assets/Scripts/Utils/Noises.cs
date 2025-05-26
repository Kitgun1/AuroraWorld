using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class Noises
    {
        public static float PerlinNoise(float x, float y) => Mathf.PerlinNoise(x, y);
    }
}