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

        public HexWorldInfo Origin;

        public HexWorldInfoProxy(HexWorldInfo origin)
        {
            Origin = origin;

            IsLand = new ReactiveProperty<bool>(Origin.IsLand);
            Elevation = new ReactiveProperty<float>(Origin.Elevation);
            Temperature = new ReactiveProperty<float>(Origin.Temperature);
            Humidity = new ReactiveProperty<float>(Origin.Humidity);

            IsLand.Skip(1).Subscribe(v => Origin.IsLand = v);
            Elevation.Skip(1).Subscribe(v => Origin.Elevation = v);
            Temperature.Skip(1).Subscribe(v => Origin.Temperature = v);
            Humidity.Skip(1).Subscribe(v => Origin.Humidity = v);
        }
    }
}