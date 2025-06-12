using System;
using System.Collections.Generic;
using AuroraWorld.Gameplay.World.Data;
using UnityEngine;

namespace AuroraWorld.Gameplay.World.Geometry
{
    public class HexEntityProxy
    {
        public Vector3Int Position { get; }
        public Vector3Int ChunkPosition => ChunkConverters.CubeToChunk(Position);
        public Vector3Int LocalInChunkPosition => ChunkConverters.CubeToLocalCube(Position);

        public HexEntityMesh HexMesh;
        public HexWorldInfoProxy WorldInfoProxy { get; }

        public HexEntity Origin { get; }

        private readonly Dictionary<DirectionType, HexEntityProxy> _neighbors = new();

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

        public HexEntityProxy[] GetNeighbors()
        {
            var result = new HexEntityProxy[6];
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = GetNeighbor((DirectionType)i);
            }

            return result;
        }

        public HexEntityProxy GetNeighbor(DirectionType dir) => _neighbors.GetValueOrDefault(dir);
        public void SetNeighbor(DirectionType dir, HexEntityProxy proxy) => _neighbors[dir] = proxy;
    }
}