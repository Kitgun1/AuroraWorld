using System;
using System.Collections.Generic;
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
            if (WorldInfoProxy.FogOfWarState.Value == FogOfWarState.Hidden)
            {
                HexMesh = new HexagonMesh()
                {
                    Colors = new Color32[] { },
                    Triangles = new int[] { },
                    Vertices = new Vector3[] { },
                    UVs = new Dictionary<string, Vector2[]>()
                    {
                        { "uv1", new Vector2[] { } },
                        { "uv2", new Vector2[] { } },
                    }
                };

                return this;
            }

            HexMesh = GeometryHexagon.InstanceUpSideMesh(Position, WorldInfoProxy);
            return this;
        }

        public HexagonProxy InitializeBordersMesh(Terrain terrain)
        {
            if (WorldInfoProxy.FogOfWarState.Value == FogOfWarState.Hidden)
            {
                return this;
            }

            GeometryHexagon.CalculateEdges(HexMesh, Position, terrain);
            GeometryHexagon.InstanceBorders(Position, HexMesh, terrain);
            return this;
        }
    }
}