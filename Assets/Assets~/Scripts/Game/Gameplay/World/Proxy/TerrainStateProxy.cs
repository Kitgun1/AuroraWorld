using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuroraWorld.App.GameResources;
using AuroraWorld.Gameplay.GameplayTime;
using AuroraWorld.Gameplay.World.Geometry;
using AuroraWorld.Gameplay.World.Root;
using AuroraWorld.Gameplay.World.Terrain;
using AuroraWorld.Gameplay.World.Terrain.Proxy;
using DI;
using ObservableCollections;
using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.World.Proxy
{
    public class TerrainStateProxy
    {
        /// <summary> Loaded Hexagons </summary>
        private readonly ObservableList<HexagonProxy> _hexagons;

        private readonly TerrainState _origin;
        private readonly Geography _geography;
        private readonly Resource<Material> _materialsResource;
        private readonly Transform _parentMesh;
        private readonly Dictionary<Vector3Int, ChunkMeshData> _chunks = new();

        private readonly HashSet<HexagonProxy> _dirtyTickHexagons = new();
        private readonly HashSet<HexagonProxy> _dirtyHexagons = new();

        public TerrainStateProxy(DIContainer container, TerrainState origin, string seed)
        {
            _materialsResource = new Resource<Material>();
            _parentMesh = container.Resolve<Transform>("ParentMeshTransform");
            _origin = origin;
            _geography = new Geography(seed);

            _hexagons = new ObservableList<HexagonProxy>();
            foreach (var hexagon in _origin.Hexagons)
            {
                AddHexagon(new HexagonProxy(hexagon));
            }
            UpdateDirtyHexagons();

            _hexagons.ObserveAdd().Subscribe(hexagon => _origin.Hexagons.Add(hexagon.Value.Origin));
            _hexagons.ObserveRemove().Subscribe(hexagon => _origin.Hexagons.Remove(hexagon.Value.Origin));

            TerrainStateRegistrations.RegisterOrLoad(this, _hexagons.ToList());
            container.Resolve<WorldStateProxy>().EntityState.UpdateEntityVisible();

            var time = container.Resolve<TimeProxy>();
            time.Tick.Where(_ => time.Ticks % 5 == 0 && _dirtyTickHexagons.Count != 0).Subscribe(_ =>
            {
                var nextDirtyHexagons = new HashSet<HexagonProxy>();

                var waterDirtyHexagons = _dirtyTickHexagons.Where(h => h.WaterSourceDistance.Value > 0).ToArray();
                foreach (var hexagon in waterDirtyHexagons)
                {
                    /* Правила осушения:
                    мы не источник воды
                    нет соседа с водой (дистанция источника != 7) на высоте >= текущей
                    Нет соседа с водой на той же высоте (water source distance < 6)
                    Нет соседа с водой на высоте выше своей (water source distance < 7)
                    */
                    if (hexagon.WaterSourceDistance.Value == 0) continue;

                    if (hexagon.CubePosition.Neighbors().All(pos =>
                        {
                            var neighbor = LoadHexagon(pos);
                            return !(neighbor.WaterSourceDistance.Value is < 7 and >= 0 &&
                                     neighbor.GeographyInfo.Elevation.Value > hexagon.GeographyInfo.Elevation.Value ||
                                     neighbor.WaterSourceDistance.Value is < 6 and >= 0 &&
                                     neighbor.GeographyInfo.Elevation.Value == hexagon.GeographyInfo.Elevation.Value);
                        }))
                    {
                        foreach (var neighbor in hexagon.CubePosition.Neighbors())
                        {
                            var neighborHexagon = LoadHexagon(neighbor);
                            nextDirtyHexagons.Add(neighborHexagon);
                        }

                        if (!ContainsLoadedHexagon(hexagon.CubePosition))
                        {
                            AddHexagon(hexagon);
                        }

                        hexagon.WaterSourceDistance.Value = -1;
                    }
                }

                var landDirtyHexagons = _dirtyTickHexagons.Where(h => !h.IsWater).ToArray();
                foreach (var hexagon in landDirtyHexagons)
                {
                    /* Правила залива:
                    Есть сосед с водой на той же высоте (water source distance < 6)
                    Есть сосед с водой на высоте выше своей (water source distance < 7)
                    */
                    int targetWaterDistance = -1;
                    Array.ForEach(hexagon.CubePosition.Neighbors(), pos =>
                    {
                        if (targetWaterDistance == 1) return;
                        var neighbor = LoadHexagon(pos);
                        if (neighbor.GeographyInfo.Elevation.Value > hexagon.GeographyInfo.Elevation.Value &&
                            neighbor.WaterSourceDistance.Value is < 7 and > 0)
                        {
                            targetWaterDistance = 1;
                        }

                        if (neighbor.GeographyInfo.Elevation.Value == hexagon.GeographyInfo.Elevation.Value &&
                            neighbor.WaterSourceDistance.Value is < 6 and > 0)
                        {
                            if (targetWaterDistance < neighbor.WaterSourceDistance.Value - 1) return;
                            targetWaterDistance = neighbor.WaterSourceDistance.Value - 1;
                        }
                    });
                    if (hexagon.WaterSourceDistance.Value != targetWaterDistance)
                    {
                        foreach (var neighbor in hexagon.CubePosition.Neighbors())
                        {
                            var neighborHexagon = LoadHexagon(neighbor);

                            nextDirtyHexagons.Add(neighborHexagon);
                        }

                        if (!ContainsLoadedHexagon(hexagon.CubePosition))
                        {
                            AddHexagon(hexagon);
                        }
                    }

                    hexagon.WaterSourceDistance.Value = targetWaterDistance;
                }

                UpdateChunks(ChunkUtils.GetModifiedChunks(_dirtyTickHexagons.Select(i => i.CubePosition).ToArray()));
                _dirtyTickHexagons.Clear();
                Array.ForEach(nextDirtyHexagons.ToArray(), h => _dirtyTickHexagons.Add(h));
            }).AddTo(_parentMesh);
        }

        public bool ContainsLoadedHexagon(Vector3Int cubePosition)
        {
            return _hexagons.Any(h => h.CubePosition == cubePosition);
        }

        public HexagonProxy LoadHexagon(Vector3Int cubePosition)
        {
            if (ContainsLoadedHexagon(cubePosition)) return _hexagons.First(h => h.CubePosition == cubePosition);

            var geographyInfo = _geography.InstanceInfo(cubePosition, out var isContinent, out var isRives);
            var waterSourceDistance = isContinent && !isRives ? -1 : 0;
            var hexagon = new Hexagon(cubePosition, geographyInfo, waterSourceDistance, FogOfWarState.Visible);
            var hexagonProxy = new HexagonProxy(hexagon);
            return hexagonProxy;
        }

        public void AddHexagon(HexagonProxy hexagonProxy)
        {
            InitializeHexagonListener(hexagonProxy);
            _hexagons.Add(hexagonProxy);
            _dirtyHexagons.Add(hexagonProxy);
        }

        public void UpdateDirtyHexagons()
        {
            UpdateChunks(_dirtyHexagons.Where(h => h != null).Select(hexagon => hexagon.ChunkPosition).ToHashSet().ToArray());
            _dirtyHexagons.Clear();
        }

        private void UpdateChunks(params Vector3Int[] chunkPositions)
        {
            foreach (var chunkPosition in chunkPositions)
            {
                var hexagons = _hexagons
                    .Where(h => h.ChunkPosition == chunkPosition)
                    .ToArray();

                if (hexagons.Length == 0) return;
                var meshes = new ConcurrentBag<HexagonMesh>();
                Parallel.ForEach(hexagons, hexagon =>
                {
                    var mesh = hexagon.CalculateMeshData(this);
                    if (mesh != null) meshes.Add(mesh);
                });
                if (meshes.Count == 0) return;

                if (!_chunks.ContainsKey(chunkPosition)) InstanceChunkObject(chunkPosition);
                var triangles = new List<int>();
                var countVertices = 0;
                foreach (var mesh in meshes)
                {
                    triangles.AddRange(mesh.Triangles.Select(t => t + countVertices));
                    countVertices += mesh.Vertices.Length;
                }
                var newMesh = new Mesh
                {
                    name = $"Chunk [{chunkPosition}]",
                    vertices = meshes.SelectMany(mesh => mesh.Vertices).ToArray(),
                    uv2 = meshes.SelectMany(h => h.UVs["uv2"]).ToArray(), // UV для граней гексов
                    colors32 = meshes.SelectMany(h => h.Colors).ToArray(),
                    triangles = triangles.ToArray()
                };

                newMesh.RecalculateNormals();
                _chunks[chunkPosition].Filter.mesh = newMesh;
                _chunks[chunkPosition].Collider.sharedMesh = newMesh;
            }
        }

        private void InitializeHexagonListener(HexagonProxy hexagonProxy)
        {
            hexagonProxy.GeographyInfo.Elevation.Skip(1).Subscribe(_ => _dirtyTickHexagons.Add(hexagonProxy));
            hexagonProxy.GeographyInfo.Elevation.Skip(1).Subscribe(_ => _dirtyHexagons.Add(hexagonProxy));
            hexagonProxy.FogOfWarState.Skip(1).Subscribe(_ => _dirtyHexagons.Add(hexagonProxy));

            //hexagonProxy.GeographyInfo.Temperature.Skip(1).Subscribe(_ => _dirtyTickHexagons.Add(hexagonProxy));
            //hexagonProxy.GeographyInfo.Humidity.Skip(1).Subscribe(_ => _dirtyTickHexagons.Add(hexagonProxy));
        }

        private void InstanceChunkObject(Vector3Int position)
        {
            var chunk = new GameObject($"[CHUNK {position}]")
            {
                transform =
                {
                    parent = _parentMesh.transform,
                }
            };
            var filter = chunk.AddComponent<MeshFilter>();
            var mesh = new Mesh
            {
                name = $"Chunk {position}"
            };
            filter.mesh = mesh;
            var renderer = chunk.AddComponent<MeshRenderer>();
            var material = _materialsResource.Load("Materials/Hexagon Map Material");
            material = new Material(material);
            renderer.material = material;

            var collider = chunk.AddComponent<MeshCollider>();
            collider.sharedMesh = mesh;

            _chunks.Add(position, new ChunkMeshData(filter, collider, renderer));
        }
    }
}