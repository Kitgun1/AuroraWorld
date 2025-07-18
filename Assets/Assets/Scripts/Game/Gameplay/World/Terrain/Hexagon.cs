using System;
using UnityEngine;

namespace AuroraWorld.Gameplay.World.Terrain
{
    [Serializable]
    public class Hexagon
    {
        public Vector3Int CubePosition;
        public GeographyInfo GeographyInfo;
        /// <summary>
        /// -1 - Is Land <br/>
        /// 0 - Is Water Source <br/>
        /// 1-7 - Water Source Distance
        /// </summary>
        public int WaterSourceDistance;
        public FogOfWarState FogOfWarState;

        public Hexagon(Vector3Int position, GeographyInfo info, int waterSourceDistance, FogOfWarState fogOfWar)
        {
            CubePosition = position;
            GeographyInfo = info;
            WaterSourceDistance = waterSourceDistance;
            FogOfWarState = fogOfWar;
        }
    }
}