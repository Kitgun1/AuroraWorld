using System.Collections.Generic;
using AuroraWorld.App.GameResources;
using AuroraWorld.Gameplay.World.Geometry;
using DI;
using ObservableCollections;
using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.World
{
    public class WorldStateProxy
    {
        public string Seed { get; }
        public ObservableDictionary<Vector3Int, HexagonProxy> Hexagons { get; } = new();

        public readonly Terrain Terrain;
        public readonly Dictionary<Vector3Int, ChunkMeshData> Chunks = new();

        public readonly WorldState Origin;

        private readonly Resource<Material> _materialsResource;
        private readonly Transform _parentMesh;

        public WorldStateProxy(DIContainer container, WorldState origin, out Vector3Int startPosition)
        {
            Origin = origin;
            Seed = Origin.Seed;

            container.RegisterInstance(this);
            Geography.SetSeed(Seed);

            Terrain = new Terrain(container);

            _materialsResource = new Resource<Material>();
            _parentMesh = container.Resolve<Transform>("ParentMeshTransform");

            Origin.Hexagons.ForEach(h =>
            {
                var hexPoxy = new HexagonProxy(h);
                hexPoxy.WorldInfoProxy.Elevation.Skip(1).Subscribe(v => Terrain.ElevationModified(hexPoxy, v));
                hexPoxy.WorldInfoProxy.IsLand.Skip(1).Subscribe(v => Terrain.LandStateModified(hexPoxy, v));
                hexPoxy.WorldInfoProxy.FogOfWar.Skip(1).Subscribe(v => Terrain.FogOfWarModified(hexPoxy, v));
                Hexagons.Add(h.Position, hexPoxy);
            });

            Hexagons.ObserveAdd().Subscribe(e => Origin.Hexagons.Add(e.Value.Value.Origin));
            Hexagons.ObserveRemove().Subscribe(e => Origin.Hexagons.Remove(e.Value.Value.Origin));

            var chunkUpdated = new HashSet<Vector3Int>();
            if (Hexagons.Count > 0)
            {
                startPosition = Vector3Int.one;
                foreach (var hexagonPair in Hexagons)
                {
                    chunkUpdated.Add(hexagonPair.Value.ChunkPosition);
                }
            }
            else
            {
                startPosition = Terrain.FindLand();
                var rangeVisible = CubeMath.Range(startPosition, 8);
                foreach (var hexagonPosition in rangeVisible)
                {
                    Terrain.AttachHexagon(hexagonPosition, out var modifiedChunks, FogOfWarState.Visible);
                    foreach (var modifiedChunk in modifiedChunks) chunkUpdated.Add(modifiedChunk);
                }
            }

            foreach (var chunkPosition in chunkUpdated)
            {
                Terrain.AttachChunkMesh(chunkPosition);
            }
        }

        public void InstanceChunkObject(Vector3Int chunkPosition)
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
                name = $"Chunk {chunkPosition}"
            };
            filter.mesh = mesh;
            var renderer = chunk.AddComponent<MeshRenderer>();
            var material = _materialsResource.Load("Materials/Hexagon Map Material");
            material = new Material(material);
            renderer.material = material;

            var collider = chunk.AddComponent<MeshCollider>();
            collider.sharedMesh = mesh;

            Chunks.Add(chunkPosition, new ChunkMeshData(filter, collider, renderer));
        }

        public struct ChunkMeshData
        {
            public readonly MeshFilter Filter;
            public readonly MeshRenderer Renderer;
            public readonly MeshCollider Collider;

            public ChunkMeshData(MeshFilter filter, MeshCollider collider, MeshRenderer renderer)
            {
                Filter = filter;
                Renderer = renderer;
                Collider = collider;
            }
        }
    }
}