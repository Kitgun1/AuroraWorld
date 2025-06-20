using System;
using System.Collections.Generic;
using System.Linq;
using AuroraWorld.Gameplay.GameplayTime;
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
                        !i.IsLand &&
                        i.Elevation > _geo.LandMinElevation &&
                        i.Elevation <= info.Elevation.Value &&
                        _dirtyHexagons.All(d => d.WorldInfoProxy.Origin != i)
                    );
                    if (info.Elevation.Value >= _geo.LandMinElevation && !hasWaterSource)
                    {
                        info.IsLand.Value = true;
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
                    var hasWaterNeighbor = neighbors.Any(i => !i.IsLand);
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
                    if (neighbors.Any(n => !n.IsLand && info.Elevation.Value <= n.Elevation))
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

        public HexagonWorldInfo GetHexagonInfo(Vector3Int cube, FogOfWarState defaultState = FogOfWarState.Hidden)
        {
            var info = _worldStateProxy.Hexagons.GetValueOrDefault(cube)?.WorldInfoProxy;
            if (info != null) return info.Origin;
            var axial = cube.ToHex();

            var elevation = _geo.GetElevation(axial);
            var hexagonInfo = new HexagonWorldInfo
            {
                Elevation = elevation,
                IsLand = _geo.LandMinElevation <= elevation,
                Humidity = _geo.GetHumidity(axial),
                Temperature = _geo.GetTemperature(axial),
                FogOfWarState = defaultState
            };

            return hexagonInfo;
        }

        public HexagonProxy AttachHexagon(Vector3Int cube, out HashSet<Vector3Int> modifiedChunks, FogOfWarState fogState = FogOfWarState.None)
        {
            var hexagonProxy = _worldStateProxy.Hexagons.GetValueOrDefault(cube);
            modifiedChunks = new HashSet<Vector3Int>();
            if (hexagonProxy != null)
            {
                if (fogState != FogOfWarState.None && hexagonProxy.WorldInfoProxy.FogOfWar.Value != fogState)
                {
                    hexagonProxy.WorldInfoProxy.FogOfWar.Value = fogState;
                    modifiedChunks.Add(hexagonProxy.ChunkPosition);
                    foreach (var neighbor in hexagonProxy.Position.Neighbors())
                    {
                        modifiedChunks.Add(ChunkUtils.CubeToChunk(neighbor));
                    }
                }

                return hexagonProxy;
            }

            if (fogState == FogOfWarState.None) fogState = FogOfWarState.Hidden;

            var hexagonEntity = new Hexagon(cube,GetHexagonInfo(cube, fogState));
            hexagonProxy = new HexagonProxy(hexagonEntity);
            hexagonProxy.WorldInfoProxy.FogOfWar.Value = fogState;
            _worldStateProxy.Hexagons.Add(cube, hexagonProxy);
            modifiedChunks.Add(hexagonProxy.ChunkPosition);
            foreach (var neighbor in hexagonProxy.Position.Neighbors())
            {
                modifiedChunks.Add(ChunkUtils.CubeToChunk(neighbor));
            }

            hexagonProxy.WorldInfoProxy.Elevation.Skip(1).Subscribe(v => ElevationModified(hexagonProxy, v));
            hexagonProxy.WorldInfoProxy.IsLand.Skip(1).Subscribe(v => LandStateModified(hexagonProxy, v));
            hexagonProxy.WorldInfoProxy.FogOfWar.Skip(1).Subscribe(v => FogOfWarModified(hexagonProxy, v));

            return hexagonProxy;
        }

        public void ElevationModified(HexagonProxy entityProxy, float elevation)
        {
            var changedChunks = ChunkUtils.GetModifiedChunks(entityProxy.Position.Neighbors());
            foreach (var chunk in changedChunks)
            {
                AttachChunkMesh(chunk);
            }

            _dirtyHexagons.Add(entityProxy);
        }

        public void LandStateModified(HexagonProxy entityProxy, bool isLand)
        {
        }

        public void FogOfWarModified(HexagonProxy entityProxy, FogOfWarState state)
        {
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
                return CubeMath.Range(land, r).All(n => GetHexagonInfo(n).IsLand);
            }
        }

        public bool ContainsHexagon(Vector3Int position) => _worldStateProxy.Hexagons.ContainsKey(position);
    }
}