using AuroraWorld.Gameplay.World.Data;
using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.World.Geometry
{
    public class HexWorldInfoProxy
    {
        public ReactiveProperty<bool> IsLand { get; }
        public ReactiveProperty<float> Elevation { get; }
        public ReactiveProperty<float> Temperature { get; }
        public ReactiveProperty<float> Humidity { get; }

        private HexWorldInfo _origin;

        public HexWorldInfoProxy(HexWorldInfo origin)
        {
            _origin = origin;

            IsLand = new ReactiveProperty<bool>(_origin.IsLand);
            Elevation = new ReactiveProperty<float>(_origin.Elevation);
            Temperature = new ReactiveProperty<float>(_origin.Temperature);
            Humidity = new ReactiveProperty<float>(_origin.Humidity);

            IsLand.Skip(1).Subscribe(v => _origin.IsLand = v);
            Elevation.Skip(1).Subscribe(v => _origin.Elevation = v);
            Temperature.Skip(1).Subscribe(v => _origin.Temperature = v);
            Humidity.Skip(1).Subscribe(v => _origin.Humidity = v);
        }
    }
}