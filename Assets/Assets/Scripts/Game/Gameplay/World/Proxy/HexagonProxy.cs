using System;
using AuroraWorld.Gameplay.World.Data;
using UnityEngine;

namespace AuroraWorld.Gameplay.World.Geometry
{
    public class HexagonProxy
    {
        public Vector3Int Position { get; }
        public Vector3Int ChunkPosition => ChunkUtils.CubeToChunk(Position);
        public Vector3Int LocalInChunkPosition => ChunkUtils.CubeToLocalCube(Position);

        public HexagonMesh HexMesh;
        public HexagonWorldInfoProxy WorldInfoProxy { get; }

        public Hexagon Origin { get; }

        public HexagonProxy(Hexagon origin, HexagonWorldInfoProxy hexagonWorldInfoProxy)
        {
            if (origin.Position.x + origin.Position.y + origin.Position.z != 0)
                throw new Exception("q + r + s is not 0!");
            Position = origin.Position;
            Origin = origin;

            WorldInfoProxy = hexagonWorldInfoProxy;
        }

        public HexagonProxy ClearMesh()
        {
            HexMesh = null;
            
            return this;
        }
        
        public HexagonProxy InitializeUpSideMesh()
        {
            HexMesh = GeometryHexagon.InstanceUpSideMesh(Position, WorldInfoProxy);
            return this;
        }

        public HexagonProxy InitializeBordersMesh(Terrain terrain)
        {
            GeometryHexagon.CalculateEdges(HexMesh, Position, terrain);
            GeometryHexagon.InstanceBorders(Position, HexMesh, terrain);
            return this;
        }
    }
}