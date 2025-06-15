using System;
using System.Collections.Generic;
using System.Linq;
using AuroraWorld.Gameplay.GameplayTime;
using AuroraWorld.Gameplay.World.Data;
using AuroraWorld.Gameplay.World.Geometry;
using DI;
using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.World
{
    public class Terrain
    {
        private readonly DIContainer _container;
        private readonly WorldStateProxy _worldStateProxy;
        private readonly GeoConfiguration _geo;
        private readonly TimeProxy _timeProxy;

        private readonly HashSet<HexagonProxy> _dirtyHexagons = new();

        public Terrain(DIContainer container)
        {
            _container = container;
            _worldStateProxy = container.Resolve<WorldStateProxy>();
            _geo = container.Resolve<GeoConfiguration>();
            _timeProxy = container.Resolve<TimeProxy>();

            _timeProxy.Tick.Skip(1).Subscribe(_ =>
            {
                if (_dirtyHexagons.Count == 0) return;
                HashSet<HexagonProxy> newDirtyHexagons = new();
                foreach (var dirtyHexagon in _dirtyHexagons.Where(e => !e.WorldInfoProxy.IsLand.Value))
                {
                    var info = dirtyHexagon.WorldInfoProxy;
                    var neighborPositions = dirtyHexagon.Position.Neighbors();
                    var neighbors = neighborPositions
                        .Select(p => GetHexagonInfo(p))
                        .ToArray();

                    // Осушаем при высоте выше уровня моря и сосед с водой не выше высоты моря
                    var hasWaterSource = neighbors.Any(i =>
                        !i.IsLand.Value &&
                        i.Elevation.Value > _geo.LandMinElevation &&
                        i.Elevation.Value <= info.Elevation.Value &&
                        _dirtyHexagons.All(d => d.WorldInfoProxy != i)
                    );
                    if (info.Elevation.Value >= _geo.LandMinElevation && !hasWaterSource)
                    {
                        info.IsLand.Value = true;
                        Debug.Log($"Осушаю... {info.IsLand.Value}");
                        Array.ForEach(neighborPositions, n =>
                        {
                            if (!ContainsHexagon(n)) return;
                            newDirtyHexagons.Add(_worldStateProxy.Hexagons[n]);
                        });
                    }
                }

                foreach (var dirtyHexagon in _dirtyHexagons.Where(e => e.WorldInfoProxy.IsLand.Value))
                {
                    var info = dirtyHexagon.WorldInfoProxy;
                    var neighborPositions = dirtyHexagon.Position.Neighbors();
                    var neighbors = neighborPositions
                        .Select(p => GetHexagonInfo(p))
                        .ToArray();

                    // Делаем водой при высоте ниже уровня моря и при наличии воды рядом
                    var hasWaterNeighbor = neighbors.Any(i => !i.IsLand.Value);
                    if (info.Elevation.Value < _geo.LandMinElevation && hasWaterNeighbor)
                    {
                        info.IsLand.Value = false;
                        Array.ForEach(neighborPositions, n =>
                        {
                            if (!ContainsHexagon(n)) return;
                            newDirtyHexagons.Add(_worldStateProxy.Hexagons[n]);
                        });
                    }

                    // Делаем водой при наличии рядом воды выше/равной текущей высоты
                    if (neighbors.Any(n => !n.IsLand.Value && info.Elevation.Value <= n.Elevation.Value))
                    {
                        info.IsLand.Value = false;
                        Array.ForEach(neighborPositions, n =>
                        {
                            if (!ContainsHexagon(n)) return;
                            newDirtyHexagons.Add(_worldStateProxy.Hexagons[n]);
                        });
                    }
                }

                var changedChunks = ChunkUtils.GetModifiedChunks(_dirtyHexagons.Select(i => i.Position).ToArray());
                foreach (var chunk in changedChunks)
                {
                    AttachChunkMesh(chunk);
                }

                _dirtyHexagons.Clear();
                Array.ForEach(newDirtyHexagons.ToArray(), h => _dirtyHexagons.Add(h));
            });
        }

        public HexagonWorldInfoProxy GetHexagonInfo(Vector3Int cube, FogOfWarState defaultState = FogOfWarState.Hidden)
        {
            var info = _worldStateProxy.Hexagons.GetValueOrDefault(cube)?.WorldInfoProxy;
            if (info != null) return info;
            var axial = cube.ToHex();

            var elevation = _geo.GetElevation(axial);
            var hexagonInfo = new HexagonWorldInfo();
            var hexagonInfoProxy = new HexagonWorldInfoProxy(hexagonInfo);
            hexagonInfoProxy.Elevation.Value = elevation;
            hexagonInfoProxy.IsLand.Value = _geo.LandMinElevation <= elevation;
            hexagonInfoProxy.Humidity.Value = _geo.GetHumidity(axial);
            hexagonInfoProxy.Temperature.Value = _geo.GetTemperature(axial);
            hexagonInfoProxy.FogOfWarState.Value = defaultState;

            return hexagonInfoProxy;
        }

        public HexagonProxy AttachHexagon(Vector3Int cube, out HashSet<Vector3Int> modifiedChunks, FogOfWarState fogState = FogOfWarState.None)
        {
            var hexEntityProxy = _worldStateProxy.Hexagons.GetValueOrDefault(cube);
            modifiedChunks = new HashSet<Vector3Int>();
            if (hexEntityProxy != null)
            {
                if (fogState != FogOfWarState.None && hexEntityProxy.WorldInfoProxy.FogOfWarState.Value != fogState)
                {
                    hexEntityProxy.WorldInfoProxy.FogOfWarState.Value = fogState;
                    modifiedChunks.Add(hexEntityProxy.ChunkPosition);
                }

                return hexEntityProxy;
            }

            if (fogState == FogOfWarState.None) fogState = FogOfWarState.Hidden;

            var hexagonEntity = new Hexagon(cube);
            hexEntityProxy = new HexagonProxy(hexagonEntity, GetHexagonInfo(cube, fogState));
            _worldStateProxy.Hexagons.Add(cube, hexEntityProxy);
            modifiedChunks.Add(hexEntityProxy.ChunkPosition);
            if (fogState != FogOfWarState.Hidden)
            {
                var neighbors = hexEntityProxy.Position.Neighbors();
                foreach (var neighborPosition in neighbors)
                {
                    modifiedChunks.Add(ChunkUtils.CubeToChunk(neighborPosition));
                    if (ContainsHexagon(neighborPosition)) continue;
                    var neighborHexagon = new Hexagon(neighborPosition);
                    var neighbor = new HexagonProxy(neighborHexagon, GetHexagonInfo(neighborPosition));
                    neighbor.WorldInfoProxy.Elevation.Skip(1).Subscribe(v => ElevationModified(neighbor, v));
                    neighbor.WorldInfoProxy.IsLand.Skip(1).Subscribe(v => LandStateModified(neighbor, v));
                    neighbor.WorldInfoProxy.FogOfWarState.Skip(1).Subscribe(v => FogOfWarModified(neighbor, v));
                    _worldStateProxy.Hexagons.Add(neighborPosition, neighbor);
                }
            }

            hexEntityProxy.WorldInfoProxy.Elevation.Skip(1).Subscribe(v => ElevationModified(hexEntityProxy, v));
            hexEntityProxy.WorldInfoProxy.IsLand.Skip(1).Subscribe(v => LandStateModified(hexEntityProxy, v));
            hexEntityProxy.WorldInfoProxy.FogOfWarState.Skip(1).Subscribe(v => FogOfWarModified(hexEntityProxy, v));

            return hexEntityProxy;

            void ElevationModified(HexagonProxy entityProxy, float elevation)
            {
                var changedChunks = ChunkUtils.GetModifiedChunks(entityProxy.Position.Neighbors());
                foreach (var chunk in changedChunks)
                {
                    AttachChunkMesh(chunk);
                }

                _dirtyHexagons.Add(entityProxy);
            }

            void LandStateModified(HexagonProxy entityProxy, bool isLand)
            {
                //_dirtyHexagons.Add(entityProxy);
            }

            void FogOfWarModified(HexagonProxy entityProxy, FogOfWarState state)
            {
                _dirtyHexagons.Add(entityProxy);
            }
        }

        public void AttachChunkMesh(Vector3Int chunkPosition)
        {
            var hexagons = _worldStateProxy.Hexagons
                .Where(h => h.Value.ChunkPosition == chunkPosition)
                .Select(h => h.Value)
                .ToArray();

            if (hexagons.Length == 0) return;
            foreach (var hexagon in hexagons)
            {
                hexagon.ClearMesh().InitializeUpSideMesh().InitializeBordersMesh(this);
            }

            // Создаем меш чанка, если его еще нет
            if (!_worldStateProxy.Chunks.ContainsKey(chunkPosition))
            {
                _worldStateProxy.InstanceChunkObject(chunkPosition);
            }

            var newMesh = new Mesh() { name = $"Chunk [{chunkPosition}]" };
            newMesh.MarkDynamic();
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
            _worldStateProxy.Chunks[chunkPosition].Filter.mesh = newMesh;
            _worldStateProxy.Chunks[chunkPosition].Collider.sharedMesh = newMesh;
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

        public bool ContainsHexagon(Vector3Int position) => _worldStateProxy.Hexagons.ContainsKey(position);
    }
}