using System;
using AuroraWorld.Gameplay.World.Data;
using AuroraWorld.Utils;
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
        public ReactiveProperty<FogOfWarHexState> FogOfWarState { get; }

        public HexWorldInfo Origin;

        public HexWorldInfoProxy(HexWorldInfo origin)
        {
            Origin = origin;

            IsLand = new ReactiveProperty<bool>(Origin.IsLand);
            Elevation = new ReactiveProperty<float>(Origin.Elevation);
            Temperature = new ReactiveProperty<float>(Origin.Temperature);
            Humidity = new ReactiveProperty<float>(Origin.Humidity);
            FogOfWarState = new ReactiveProperty<FogOfWarHexState>(Origin.FogOfWarState);

            IsLand.Skip(1).Subscribe(v => Origin.IsLand = v);
            Elevation.Skip(1).Subscribe(v =>
            {
                Origin.Elevation = Mathf.Clamp01(Mathf.RoundToInt(v / GeometryHexagon.ELEVATION_STEP) *
                                                 GeometryHexagon.ELEVATION_STEP);
            });
            Temperature.Skip(1).Subscribe(v => Origin.Temperature = v);
            Humidity.Skip(1).Subscribe(v => Origin.Humidity = v);
            FogOfWarState.Skip(1).Subscribe(v => Origin.FogOfWarState = v);
        }

        public Color32 GetBiomeColor(int seedGamma)
        {
            Color32 color;
            var random = new System.Random(seedGamma);
            var gamma = (int)Mathf.Lerp(-60, 60, random.Next(0, 1000) / 1000f);
            if (IsLand.Value)
                color = new Color32(0, 155, 0, 255).AddGamma(gamma);
            else color = new Color32(28, 169, 201, 255).AddGamma(gamma);
            color.a = FogOfWarState.Value switch
            {
                FogOfWarHexState.Visible => 255,
                FogOfWarHexState.Visited => 70,
                FogOfWarHexState.Hidden => 0,
                _ => throw new ArgumentOutOfRangeException(nameof(FogOfWarState), FogOfWarState.Value, null)
            };

            return color;
        }
    }
}