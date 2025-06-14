using System;
using System.Collections.Generic;
using System.Linq;
using AuroraWorld.Gameplay.World.Data;
using AuroraWorld.Gameplay.World.Geometry;
using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.World
{
    public class WorldTerrain
    {
        private readonly WorldStateProxy _worldStateProxy;
        private readonly GeoConfiguration _geo;

        public WorldTerrain(WorldStateProxy worldStateProxy, GeoConfiguration geo)
        {
            _worldStateProxy = worldStateProxy;
            _geo = geo;
        }

        public HexWorldInfoProxy GetHexagonInfo(Vector3Int cube, FogOfWarState defaultFogState = FogOfWarState.Hidden)
        {
            var info = _worldStateProxy.Hexagons.GetValueOrDefault(cube)?.WorldInfoProxy;
            if (info != null) return info;
            var axial = cube.ToHex();

            var elevation = _geo.GetElevation(axial);
            var hexagonInfo = new HexWorldInfo();
            var hexagonInfoProxy = new HexWorldInfoProxy(hexagonInfo);
            hexagonInfoProxy.Elevation.Value = elevation;
            hexagonInfoProxy.IsLand.Value = _geo.LandMinElevation <= elevation;
            hexagonInfoProxy.Humidity.Value = _geo.GetHumidity(axial);
            hexagonInfoProxy.Temperature.Value = _geo.GetTemperature(axial);
            hexagonInfoProxy.FogOfWarState.Value = defaultFogState;

            return hexagonInfoProxy;
        }

        public HexEntityProxy AttachHexagon(Vector3Int cube, FogOfWarHexState fogState = FogOfWarHexState.Hidden)
        {
            // Создаем меш чанка, если его нет
            var chunkPosition = ChunkConverters.CubeToChunk(cube);
            if (!_worldStateProxy.Chunks.ContainsKey(chunkPosition))
            {
                _worldStateProxy.InstanceChunkObject(chunkPosition);
            }

            // Добавляем данные, если шестиугольник новый
            var hexagonEntityProxy = _worldStateProxy.Hexagons.GetValueOrDefault(cube);
            if (!_worldStateProxy.Hexagons.ContainsKey(cube))
            {
                var hexagonEntity = new HexEntity(cube);
                hexagonEntityProxy = new HexEntityProxy(hexagonEntity, GetHexagonInfo(cube, fogState));

                // Обработка изменений
                hexagonEntityProxy.WorldInfoProxy.Elevation.Skip(1).Subscribe(v =>
                {
                    var info = hexagonEntityProxy.WorldInfoProxy;
                    var oldIsLand = info.IsLand.Value;
                    info.IsLand.Value = !hexagonEntityProxy.Position.Neighbors()
                        .Any(n =>
                        {
                            var neighborInfo = GetHexagonInfo(n);
                            return !neighborInfo.IsLand.Value && neighborInfo.Elevation.Value >= v;
                        });

                    hexagonEntityProxy.ClearMesh().InitializeUpSideMesh().InitializeBordersMesh(this);
                    var neighborsPosition = hexagonEntityProxy.Position.Neighbors();
                    foreach (var neighborPosition in neighborsPosition)
                    {
                        var neighbor = _worldStateProxy.Hexagons.GetValueOrDefault(neighborPosition);
                        neighbor.ClearMesh().InitializeUpSideMesh().InitializeBordersMesh(this);
                    }
                    // if(info.IsLand.Value == oldIsLand)
                    // TODO: Обновляем весь чанк
                });

                hexagonEntityProxy.WorldInfoProxy.IsLand.Skip(1).Subscribe(v =>
                {
                    var neighborsPosition = hexagonEntityProxy.Position.Neighbors();
                    foreach (var neighborPosition in neighborsPosition)
                    {
                        var neighbor = _worldStateProxy.Hexagons.GetValueOrDefault(neighborPosition) ??
                                       AttachHexagon(neighborPosition);

                        var info = neighbor.WorldInfoProxy;
                        info.IsLand.Value = !neighbor.Position.Neighbors()
                            .Any(n =>
                            {
                                var neighborInfo = GetHexagonInfo(n);
                                return !neighborInfo.IsLand.Value &&
                                       neighborInfo.Elevation.Value >= info.Elevation.Value;
                            });
                    }

                    hexagonEntityProxy.ClearMesh().InitializeUpSideMesh().InitializeBordersMesh(this);
                    // TODO: Обновляем весь чанк
                });

                hexagonEntityProxy.WorldInfoProxy.FogOfWarState.Skip(1).Subscribe(_ =>
                {
                    hexagonEntityProxy.ClearMesh().InitializeUpSideMesh().InitializeBordersMesh(this);
                    var neighborsPosition = hexagonEntityProxy.Position.Neighbors();
                    foreach (var neighborPosition in neighborsPosition)
                    {
                        var neighbor = _worldStateProxy.Hexagons.GetValueOrDefault(neighborPosition);
                        if (neighbor != null)
                        {
                            neighbor.ClearMesh().InitializeUpSideMesh().InitializeBordersMesh(this);
                        }
                    }
                    // TODO: Обновляем весь чанк
                });

                _worldStateProxy.Hexagons.Add(cube, hexagonEntityProxy);
            }

            var hexagons = _worldStateProxy.Hexagons
                .Where(h => h.Value.ChunkPosition == chunkPosition)
                .Select(h => h.Value)
                .ToArray();

            foreach (var hexagon in hexagons)
            {
                hexagon.ClearMesh().InitializeUpSideMesh().InitializeBordersMesh(this);
            }

            var newMesh = new Mesh() { name = $"Chunk [{chunkPosition}]" };
            newMesh.vertices = hexagons.SelectMany(h => h.HexMesh.Vertices).ToArray();
            newMesh.uv2 = hexagons.SelectMany(h => h.HexMesh.UVs["uv2"]).ToArray(); // UV для граней гексов
            newMesh.colors32 = hexagons.SelectMany(h => h.HexMesh.Colors).ToArray();
            var triangles = new List<int>();
            var countVertices = 0;
            foreach (var hexagon in hexagons)
            {
                triangles.AddRange(hexagon.HexMesh.Triangles.Select(t => t + countVertices));
                countVertices += hexagon.HexMesh.Vertices.Length;
            }

            newMesh.triangles = triangles.ToArray();

            newMesh.RecalculateNormals();
            _worldStateProxy.Chunks[chunkPosition].Collider.sharedMesh = newMesh;
            _worldStateProxy.Chunks[chunkPosition].Filter.mesh = newMesh;

            return hexagonEntityProxy;
        }

        public Vector3Int FindLand()
        {
            var maxSteps = 150;
            var startHexagonPosition = Vector3Int.zero;
            int radius = 0;
            while (startHexagonPosition == Vector3Int.zero)
            {
                if (maxSteps <= 0)
                {
                    throw new Exception("Not Founded land!");
                }

                if (radius == 0)
                {
                    if (CheckRadiusLand(Vector3Int.zero))
                    {
                        startHexagonPosition = Vector3Int.zero;
                        break;
                    }
                }

                var ring = CubeMath.Ring(Vector3Int.zero, radius);
                for (var i = 0; i < ring.Length; i += 3)
                {
                    var cubePosition = ring[i];
                    if (CheckRadiusLand(cubePosition))
                    {
                        startHexagonPosition = cubePosition;
                        break;
                    }
                }

                maxSteps--;
                radius += 5;
            }

            return startHexagonPosition;

            bool CheckRadiusLand(Vector3Int land, int r = 1)
            {
                return CubeMath.Range(land, r).All(n => GetHexagonInfo(n).IsLand.Value);
            }
        }
    }
}