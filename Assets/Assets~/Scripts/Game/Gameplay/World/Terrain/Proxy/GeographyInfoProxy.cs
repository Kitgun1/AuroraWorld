using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.World.Terrain.Proxy
{
    public class GeographyInfoProxy
    {
        public readonly ReactiveProperty<int> Elevation;
        public readonly ReactiveProperty<float> Temperature;
        public readonly ReactiveProperty<float> Vegetation;
        public readonly ReactiveProperty<float> Humidity;
        public readonly ReactiveProperty<TerrainBiome> TerrainBiome;

        private readonly GeographyInfo _origin;

        public GeographyInfoProxy(GeographyInfo origin)
        {
            _origin = origin;

            Elevation = new ReactiveProperty<int>(_origin.Elevation);
            Temperature = new ReactiveProperty<float>(_origin.Temperature);
            Vegetation = new ReactiveProperty<float>(_origin.Vegetation);
            Humidity = new ReactiveProperty<float>(_origin.Humidity);
            TerrainBiome = new ReactiveProperty<TerrainBiome>(_origin.TerrainBiome);

            Elevation.Skip(1).Subscribe(newValue => _origin.Elevation = newValue);
            Temperature.Skip(1).Subscribe(newValue => _origin.Temperature = Mathf.Clamp01(newValue));
            Vegetation.Skip(1).Subscribe(newValue => _origin.Vegetation = Mathf.Clamp01(newValue));
            Humidity.Skip(1).Subscribe(newValue => _origin.Humidity = Mathf.Clamp01(newValue));
            TerrainBiome.Skip(1).Subscribe(newValue => _origin.TerrainBiome = newValue);
        }
    }
}