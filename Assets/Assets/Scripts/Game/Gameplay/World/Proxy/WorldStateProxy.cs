using System;
using System.Collections.Generic;
using System.Linq;
using AuroraWorld.Gameplay.World.Data;
using DI;
using ObservableCollections;
using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.World.Geometry
{
    public class WorldStateProxy
    {
        private readonly Transform _parentMesh;
        private readonly GeoConfiguration _geoConfiguration;
        private readonly Dictionary<Vector3Int, MeshFilter> _chunks = new();

        public ObservableDictionary<Vector3Int, HexEntityProxy> Hexagons { get; } = new();
        public ObservableList<MergedHexEntityGroupProxy> MergedHexagon { get; } = new();

        public WorldState Origin;

        public WorldStateProxy(DIContainer container, WorldState origin, string seed)
        {
            Origin = origin;
            _parentMesh = container.Resolve<Transform>("ParentMeshTransform");
            _geoConfiguration = container.Resolve<GeoConfiguration>();
            Geography.SetSeed(seed);

            Origin.Hexagons.ForEach(h => Hexagons.Add(h.Position, new HexEntityProxy(h, _geoConfiguration)
                .InitializeNeighbors(this)
                .InitializeMesh()));
            Origin.MergedHexagons.ForEach(group => MergedHexagon.Add(new MergedHexEntityGroupProxy(group)));

            Hexagons.ObserveAdd().Subscribe(e => Origin.Hexagons.Add(e.Value.Value.Origin));
            Hexagons.ObserveRemove().Subscribe(e => Origin.Hexagons.Remove(e.Value.Value.Origin));

            MergedHexagon.ObserveAdd().Subscribe(e => Origin.MergedHexagons.Add(e.Value.Origin));
            MergedHexagon.ObserveRemove().Subscribe(e => Origin.MergedHexagons.Remove(e.Value.Origin));


            var startPosition = FindNearLand();
            var range = CubeMath.Range(startPosition, 8);
            //range = new[] { Vector3Int.zero, new Vector3Int(1, -1, 0) };

            foreach (var cubePosition in range)
            {
                AttachHexagon(cubePosition);
            }
        }

        public void InstanceChunk(Vector3Int chunkPosition)
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var colors = new List<Color>();
            var chunkCenter = ChunkConverters.ChunkCenter(chunkPosition);
            var worldCubes = CubeMath.Range(chunkCenter, ChunkConverters.CHUNK_RADIUS);

            foreach (var cube in worldCubes)
            {
                var entityProxy = InstanceHexagonEntityProxy(cube)
                    .InitializeNeighbors(this)
                    .InitializeMesh();
                triangles.AddRange(entityProxy.EntityMesh.Triangles.Select(t => t + vertices.Count));
                colors.AddRange(entityProxy.EntityMesh.Colors);
                vertices.AddRange(entityProxy.EntityMesh.Vertices);
            }

            var mesh = new Mesh
            {
                name = $"Chunk {chunkPosition}",
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray(),
                colors = colors.ToArray()
            };
            mesh.RecalculateNormals();

            if (!_chunks.ContainsKey(chunkPosition)) InstanceChunkObject(chunkPosition);
            _chunks[chunkPosition].mesh = mesh;
        }

        public void AttachHexagon(Vector3Int position)
        {
            var chunkPosition = ChunkConverters.CubeToChunk(position);

            if (!_chunks.ContainsKey(chunkPosition)) InstanceChunkObject(chunkPosition);

            var chunkMesh = _chunks[chunkPosition].mesh;
            var hexProxy = InstanceHexagonEntityProxy(position)
                .InitializeNeighbors(this)
                .InitializeMesh();

            var successAdded = Hexagons.TryAdd(hexProxy.Position, hexProxy);
            if (!successAdded) Hexagons[hexProxy.Position] = hexProxy;

            var newTriangles = hexProxy.EntityMesh.Triangles.Select(t => t + chunkMesh.vertices.Length);
            var newVertices = hexProxy.EntityMesh.Vertices;
            var newColors = hexProxy.EntityMesh.Colors;

            var newMesh = new Mesh() { name = $"Chunk position {chunkPosition}" };
            var mergedVertices = chunkMesh.vertices.ToList();
            mergedVertices.AddRange(newVertices);
            newMesh.vertices = mergedVertices.ToArray();

            var mergedColors = chunkMesh.colors.ToList();
            mergedColors.AddRange(newColors);
            newMesh.colors = mergedColors.ToArray();

            var mergedTriangles = chunkMesh.triangles.ToList();
            mergedTriangles.AddRange(newTriangles);
            newMesh.triangles = mergedTriangles.ToArray();

            newMesh.RecalculateNormals();
            _chunks[chunkPosition].mesh = newMesh;
        }

        private HexEntityProxy InstanceHexagonEntityProxy(Vector3Int cubePosition)
        {
            var hexEntity = new HexEntity(cubePosition);
            var hexEntityProxy = new HexEntityProxy(hexEntity, _geoConfiguration);

            return hexEntityProxy;
        }

        private void InstanceChunkObject(Vector3Int chunkPosition)
        {
            var chunk = new GameObject($"[CHUNK {chunkPosition}]")
            {
                transform =
                {
                    parent = _parentMesh.transform,
                }
            };
            var filter = chunk.AddComponent<MeshFilter>();
            var mesh = new Mesh
            {
                name = $"Chunk {chunkPosition}",
                vertices = { },
                triangles = { },
                colors = { }
            };
            filter.mesh = mesh;
            var renderer = chunk.AddComponent<MeshRenderer>();
            renderer.material = Resources.Load<Material>("Vertex Material");

            _chunks.Add(chunkPosition, filter);
        }

        private Vector3Int FindNearLand(int maxSteps = 100)
        {
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
                    var hexagonEntityProxy = InstanceHexagonEntityProxy(Vector3Int.zero);
                    if (hexagonEntityProxy.WorldInfoProxy.IsLand.Value)
                    {
                        startHexagonPosition = hexagonEntityProxy.Position;
                        break;
                    }
                }

                var ring = CubeMath.Ring(Vector3Int.zero, radius);
                foreach (var cubePosition in ring)
                {
                    var hexagonEntityProxy = InstanceHexagonEntityProxy(cubePosition);
                    if (hexagonEntityProxy.WorldInfoProxy.IsLand.Value)
                    {
                        startHexagonPosition = hexagonEntityProxy.Position;
                        break;
                    }
                }

                maxSteps--;
                radius += 3;
            }

            return startHexagonPosition;
        }
    }
}