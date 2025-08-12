using System;
using System.Collections.Generic;
using System.Linq;
using AuroraWorld.Gameplay.World.Geometry;
using AuroraWorld.Gameplay.World.Proxy;
using AuroraWorld.Gameplay.World.Terrain.Proxy;
using UnityEngine;

namespace AuroraWorld.Gameplay.World.Root
{
    public static class TerrainStateRegistrations
    {
        public static void RegisterOrLoad(TerrainStateProxy terrain, List<HexagonProxy> hexagons)
        {
            if (hexagons.Count == 0)
            {
                var startPosition = FindLand(terrain);
                var rangeVisible = CubeMath.Range(startPosition, 8);
                foreach (var hexagonPosition in rangeVisible)
                {
                    var hexagonProxy = terrain.LoadHexagon(hexagonPosition);
                    terrain.AddHexagon(hexagonProxy);
                }
                terrain.UpdateDirtyHexagons();
            }
        }

        private static Vector3Int FindLand(TerrainStateProxy terrain)
        {
            var maxSteps = 50;
            var startHexagonPosition = Vector3Int.zero;
            int radius = 0;
            while (startHexagonPosition == Vector3Int.zero)
            {
                if (maxSteps <= 0)
                {
                    throw new Exception("Not Founded land!");
                }

                var ring = CubeMath.Ring(Vector3Int.zero, radius);
                for (var i = 0; i < ring.Length; i += 3)
                {
                    var cubePosition = ring[i];
                    if (!CheckRadiusLand(cubePosition)) continue;
                    startHexagonPosition = cubePosition;
                    break;
                }

                maxSteps--;
                radius += 5;
            }

            return startHexagonPosition;

            bool CheckRadiusLand(Vector3Int land, int r = 1)
            {
                return CubeMath.Range(land, r).All(n => !terrain.LoadHexagon(n).IsWater);
            }
        }
    }
}