using AuroraWorld.Gameplay.World.Geometry;
using AuroraWorld.Gameplay.World.Proxy;
using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.World.Terrain.Proxy
{
    public class HexagonProxy
    {
        public readonly Vector3Int CubePosition;
        public readonly GeographyInfoProxy GeographyInfo;
        /// <summary>
        /// -1 - Is Land <br/>
        /// 0 - Is Water Source <br/>
        /// 1-7 - Water Source Distance
        /// </summary>
        public readonly ReactiveProperty<int> WaterSourceDistance;
        public readonly ReactiveProperty<FogOfWarState> FogOfWarState;
        
        public readonly Hexagon Origin;

        public Vector3Int ChunkPosition => ChunkUtils.CubeToChunk(CubePosition);
        public Vector2Int AxialPosition => CubePosition.ToHex();
        public Vector3 WorldPosition => CubePosition.CubeToWorld(GeographyInfo.Elevation.Value);
        public bool IsWater => WaterSourceDistance.Value >= 0;

        public HexagonProxy(Hexagon origin)
        {
            Origin = origin;

            CubePosition = Origin.CubePosition;
            GeographyInfo = new GeographyInfoProxy(Origin.GeographyInfo);
            WaterSourceDistance = new ReactiveProperty<int>(Origin.WaterSourceDistance);
            FogOfWarState = new ReactiveProperty<FogOfWarState>(Origin.FogOfWarState);

            WaterSourceDistance.Skip(1).Subscribe(newValue => Origin.WaterSourceDistance = newValue);
            FogOfWarState.Skip(1).Subscribe(newValue => Origin.FogOfWarState = newValue);
        }

        public HexagonMesh CalculateMeshData(TerrainStateProxy terrain)
        {
            if (!terrain.ContainsLoadedHexagon(CubePosition) || FogOfWarState.Value == World.Terrain.FogOfWarState.Hide)
            {
                return null;
            }
            var meshData = GeometryHexagon.CalculateUpSideMesh(CubePosition, GeographyInfo);
            meshData = CalculateBordersMesh(meshData, terrain);
            return meshData;
        }

        private HexagonMesh CalculateBordersMesh(HexagonMesh meshData, TerrainStateProxy terrain)
        {
            GeometryHexagon.CalculateEdges(meshData, CubePosition, terrain);
            meshData = GeometryHexagon.CalculateBordersMesh(CubePosition, meshData, terrain);
            return meshData;
        }
    }
}