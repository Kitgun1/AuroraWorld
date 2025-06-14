using System;
using AuroraWorld.Gameplay.World.Data;
using UnityEngine;

namespace AuroraWorld.Gameplay.World.Geometry
{
    public class HexEntityProxy
    {
        public Vector3Int Position { get; }
        public Vector3Int ChunkPosition => ChunkUtils.CubeToChunk(Position);
        public Vector3Int LocalInChunkPosition => ChunkUtils.CubeToLocalCube(Position);

        public HexEntityMesh HexMesh;
        public HexWorldInfoProxy WorldInfoProxy { get; }

        public HexEntity Origin { get; }

        public HexEntityProxy(HexEntity origin, HexWorldInfoProxy hexWorldInfoProxy)
        {
            if (origin.Position.x + origin.Position.y + origin.Position.z != 0)
                throw new Exception("q + r + s is not 0!");
            Position = origin.Position;
            Origin = origin;

            WorldInfoProxy = hexWorldInfoProxy;
        }

        public HexEntityProxy ClearMesh()
        {
            HexMesh = null;
            
            return this;
        }
        
        public HexEntityProxy InitializeUpSideMesh()
        {
            HexMesh = GeometryHexagon.InstanceUpSideMesh(Position, WorldInfoProxy);
            return this;
        }

        public HexEntityProxy InitializeBordersMesh(WorldTerrain terrain)
        {
            GeometryHexagon.CalculateEdges(HexMesh, Position, terrain);
            GeometryHexagon.InstanceBorders(Position, HexMesh, terrain);
            return this;
        }
    }
}