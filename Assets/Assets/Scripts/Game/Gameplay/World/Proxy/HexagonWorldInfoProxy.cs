using System;
using AuroraWorld.Gameplay.World.Data;
using AuroraWorld.Utils;
using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.World.Geometry
{
    public class HexagonWorldInfoProxy
    {
        public ReactiveProperty<bool> IsLand { get; }
        public ReactiveProperty<int> Elevation { get; }
        public ReactiveProperty<float> Temperature { get; }
        public ReactiveProperty<float> Humidity { get; }
        public ReactiveProperty<FogOfWarState> FogOfWarState { get; }

        public HexagonWorldInfo Origin;

        public HexagonWorldInfoProxy(HexagonWorldInfo origin)
        {
            Origin = origin;

            IsLand = new ReactiveProperty<bool>(Origin.IsLand);
            Elevation = new ReactiveProperty<int>(Origin.Elevation);
            Temperature = new ReactiveProperty<float>(Origin.Temperature);
            Humidity = new ReactiveProperty<float>(Origin.Humidity);
            FogOfWarState = new ReactiveProperty<FogOfWarState>(Origin.FogOfWarState);

            IsLand.Skip(1).Subscribe(v => Origin.IsLand = v);
            Elevation.Skip(1).Subscribe(v =>
            {
                Origin.Elevation = Mathf.RoundToInt(v / GeometryHexagon.ELEVATION_STEP);
            });
            Temperature.Skip(1).Subscribe(v => Origin.Temperature = v);
            Humidity.Skip(1).Subscribe(v => Origin.Humidity = v);
            FogOfWarState.Skip(1).Subscribe(v => Origin.FogOfWarState = v);
        }

        public Color32 GetBiomeColor(Vector3Int cube)
        {
            Color32 color;
            var noise = Mathf.PerlinNoise(cube.x * 0.9f, cube.y * 0.9f);
            var gamma = (int)Mathf.Lerp(-30, 30, noise);
            if (IsLand.Value)
                color = new Color32(0, 155, 0, 255).AddGamma(gamma);
            else color = new Color32(28, 169, 201, 255).AddGamma(gamma);
            color.a = FogOfWarState.Value switch
            {
                Data.FogOfWarState.Visible => 255,
                Data.FogOfWarState.Visited => 70,
                Data.FogOfWarState.Hidden => 0,
                _ => throw new ArgumentOutOfRangeException(nameof(FogOfWarState), FogOfWarState.Value, null)
            };

            return color;
        }
    }
}