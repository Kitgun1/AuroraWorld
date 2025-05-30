using System;
using System.Collections.Generic;
using System.Linq;
using AuroraWorld.Gameplay.World.Data;
using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.World.Geometry
{
    public class HexEntityProxy
    {
        public Vector3Int Position { get; }
        public Vector3Int ChunkPosition => ChunkConverters.CubeToChunk(Position);
        public Vector3Int LocalInChunkPosition => ChunkConverters.CubeToLocalCube(Position);

        public HexEntityMesh EntityMesh;
        public HexWorldInfoProxy WorldInfoProxy { get; }

        public HexEntity Origin { get; }

        private readonly Dictionary<DirectionType, HexEntityProxy> _neighbors = new();
        private readonly GeoConfiguration _configuration;

        public HexEntityProxy(HexEntity origin, GeoConfiguration configuration)
        {
            if (origin.Position.x + origin.Position.y + origin.Position.z != 0)
                throw new Exception("q + r + s is not 0!");
            Position = origin.Position;
            _configuration = configuration;
            Origin = origin;

            WorldInfoProxy = configuration.GetHexagonInfo(Position.ToHex());

            WorldInfoProxy.IsLand.Skip(1).Subscribe(isLand => Origin.WorldInfo.IsLand = isLand);
            WorldInfoProxy.Elevation.Skip(1).Subscribe(elevation => Origin.WorldInfo.Elevation = elevation);
            WorldInfoProxy.Temperature.Skip(1).Subscribe(temperature => Origin.WorldInfo.Temperature = temperature);
            WorldInfoProxy.Humidity.Skip(1).Subscribe(humidity => Origin.WorldInfo.Humidity = humidity);
        }

        public HexEntityProxy InitializeNeighbors(WorldStateProxy world)
        {
            for (int i = 0; i < 6; i++)
            {
                var neighborPos = Position.Neighbor((DirectionType)i);
                if (!world.Hexagons.TryGetValue(neighborPos, out var neighbor)) continue;
                neighbor.SetNeighbor((DirectionType)(i >= 3 ? i - 3 : i + 3), this);
                SetNeighbor((DirectionType)i, neighbor);
            }

            return this;
        }

        public HexEntityProxy InitializeMesh()
        {
            var neighbors = GetNeighbors();
            var neighborsInfoProxy = neighbors.Select(p => p?.WorldInfoProxy).ToArray();
            EntityMesh = Hexagon.CalculateUpSideAndEdges(_configuration, Position, WorldInfoProxy, neighbors);
            EntityMesh = Hexagon.CalculateHexagonBorders(_configuration, Position, EntityMesh, neighborsInfoProxy);

            return this;
        }

        public HexEntityProxy[] GetNeighbors()
        {
            var result = new HexEntityProxy[6];
            for (var i = 0; i < result.Length; i++)
            {
                var success = _neighbors.TryGetValue((DirectionType)i, out var neighbor);
                result[i] = success ? neighbor : null;
            }

            return result;
        }

        public HexEntityProxy GetNeighbor(DirectionType dir) => _neighbors[dir] != null ? _neighbors[dir] : null;

        public HexEntityProxy SetNeighbor(DirectionType dir, HexEntityProxy proxy) => _neighbors[dir] = proxy;
    }
}