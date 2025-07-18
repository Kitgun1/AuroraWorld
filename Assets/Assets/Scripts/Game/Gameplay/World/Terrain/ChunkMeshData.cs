using UnityEngine;

namespace AuroraWorld.Gameplay.World.Terrain
{
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