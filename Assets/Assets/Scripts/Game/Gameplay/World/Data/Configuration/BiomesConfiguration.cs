using UnityEngine;

namespace AuroraWorld.Gameplay.World.Data
{
    [CreateAssetMenu(fileName = "new Biome Settings", menuName = "Aurora World/World/Biomes Settings", order = 0)]
    public class BiomesConfiguration : ScriptableObject
    {
        public LocalGeoSettings[] BiomeSettings;
    }
}