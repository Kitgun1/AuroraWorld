using System.Linq;
using AuroraWorld.Core.Mathematics.Base;
using UnityEngine;
using static UnityEngine.Mathf;

namespace AuroraWorld.Core.Mathematics
{
    public static class CMath
    {
        public static readonly CubeVector[] Directions =
        {
            new(0, 1, -1), // top-right 
            new(-1, 1, 0), // top-left 
            new(-1, 0, 1), // left 
            new(0, -1, 1), // down-left
            new(1, -1, 0), // down-right
            new(1, 0, -1), // right
        };

        public static CubeVector ToCubeSpace(this DirectionType direction) => direction is DirectionType.None ? CubeVector.zero : Directions[(int)direction];
        
        public static CubeVector Neighbor(this CubeVector cube, CubeVector cubeDir) => cube + cubeDir;
        public static CubeVector Neighbor(this CubeVector cube, DirectionType dir) => cube + dir.ToCubeSpace();
        public static CubeVector[] Neighbors(this Vector3Int cube) => Directions.Select(direction => direction + cube).ToArray();
        
        public static int Distance(Vector3Int a, Vector3Int b) => (Abs(a.x - b.x) + Abs(a.y - b.y) + Abs(a.z - b.z)) / 2;
        public static Vector3 CubeSpaceLerp(Vector3Int a, Vector3Int b, float t) => new(Lerp(a.x, b.x, t), Lerp(a.y, b.y, t), Lerp(a.z, b.z, t));
        public static Vector3Int Round(Vector3 cube)
        {
            var q = RoundToInt(cube.x);
            var r = RoundToInt(cube.y);
            var s = RoundToInt(cube.z);

            var qDiff = Abs(q - cube.x);
            var rDiff = Abs(r - cube.y);
            var sDiff = Abs(s - cube.z);

            if (qDiff > rDiff && qDiff > sDiff) q = -r - s;
            else if (rDiff > sDiff) r = -q - s;
            else s = -q - s;
            return new Vector3Int(q, r, s);
        }
    }
}