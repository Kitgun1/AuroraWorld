using System.Collections.Generic;
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
        public ObservableDictionary<Vector3Int, HexEntityProxy> Hexagons { get; } = new();

        public WorldState Origin;
        public readonly WorldTerrain WorldTerrain;
        public readonly Dictionary<Vector3Int, ChunkMeshData> Chunks = new();

        public WorldStateProxy(DIContainer container, WorldState origin, string seed, out Vector3Int startPosition)
        {
            Origin = origin;
            _parentMesh = container.Resolve<Transform>("ParentMeshTransform");
            WorldTerrain = new WorldTerrain(this, container.Resolve<GeoConfiguration>());
            Geography.SetSeed(seed);

            Origin.Hexagons.ForEach(h =>
            {
                // TODO: Init neighbors
                Hexagons.Add(h.Position, new HexEntityProxy(h, WorldTerrain.GetHexagonInfo(h.Position)));
            });

            Hexagons.ObserveAdd().Subscribe(e => Origin.Hexagons.Add(e.Value.Value.Origin));
            Hexagons.ObserveRemove().Subscribe(e => Origin.Hexagons.Remove(e.Value.Value.Origin));

            startPosition = WorldTerrain.FindLand();
            var rangeVisible = CubeMath.Range(startPosition, 8);
            var chunkUpdated = new HashSet<Vector3Int>();
            foreach (var hexagonPosition in rangeVisible)
            {
                WorldTerrain.AttachHexagon(hexagonPosition, out var modifiedChunks, FogOfWarState.Visible);
                foreach (var modifiedChunk in modifiedChunks) chunkUpdated.Add(modifiedChunk);
            }

            foreach (var chunkPosition in chunkUpdated)
            {
                WorldTerrain.AttachChunkMesh(chunkPosition);
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
            var material = Resources.Load<Material>("Hexagon Map Material");
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