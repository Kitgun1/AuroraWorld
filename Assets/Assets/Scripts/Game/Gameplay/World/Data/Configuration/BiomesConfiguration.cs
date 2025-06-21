using UnityEngine;

namespace AuroraWorld.Gameplay.World
{
    [CreateAssetMenu(fileName = "new Biome Settings", menuName = "Aurora World/World/Biomes Settings", order = 0)]
    public class BiomesConfiguration : ScriptableObject
    {
        public LocalGeoSettings[] BiomeSettings;
    }
}