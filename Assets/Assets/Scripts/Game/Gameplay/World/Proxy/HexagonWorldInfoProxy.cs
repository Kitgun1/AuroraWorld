using System;
using AuroraWorld.Gameplay.World.Geometry;
using AuroraWorld.Utils;
using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.World
{
    public class HexagonWorldInfoProxy
    {
        public ReactiveProperty<bool> IsLand { get; }
        public ReactiveProperty<int> Elevation { get; }
        public ReactiveProperty<float> Temperature { get; }
        public ReactiveProperty<float> Humidity { get; }
        public ReactiveProperty<FogOfWarState> FogOfWar { get; }

        public readonly HexagonWorldInfo Origin;

        public HexagonWorldInfoProxy(HexagonWorldInfo origin)
        {
            Origin = origin;

            IsLand = new ReactiveProperty<bool>(Origin.IsLand);
            Elevation = new ReactiveProperty<int>(Origin.Elevation);
            Temperature = new ReactiveProperty<float>(Origin.Temperature);
            Humidity = new ReactiveProperty<float>(Origin.Humidity);
            FogOfWar = new ReactiveProperty<FogOfWarState>(Origin.FogOfWarState);

            IsLand.Skip(1).Subscribe(v => Origin.IsLand = v);
            Elevation.Skip(1).Subscribe(v => Origin.Elevation = v);
            Temperature.Skip(1).Subscribe(v => Origin.Temperature = v);
            Humidity.Skip(1).Subscribe(v => Origin.Humidity = v);
            FogOfWar.Skip(1).Subscribe(v => Origin.FogOfWarState = v);
        }

        public Color32 GetBiomeColor(Vector3Int cube)
        {
            Color32 color;
            var noise = Mathf.PerlinNoise(cube.x * 0.9f, cube.y * 0.9f);
            var gamma = (int)Mathf.Lerp(-30, 30, noise);
            if (IsLand.Value)
                color = new Color32(0, 155, 0, 255).AddGamma(gamma);
            else color = new Color32(28, 169, 201, 255).AddGamma(gamma);
            color.a = FogOfWar.Value switch
            {
                FogOfWarState.Visible => 255,
                FogOfWarState.Visited => 70,
                FogOfWarState.Hidden => 0,
                _ => throw new ArgumentOutOfRangeException(nameof(FogOfWar), FogOfWar.Value, null)
            };

            return color;
        }
    }
}