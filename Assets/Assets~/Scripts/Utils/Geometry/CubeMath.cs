using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;

namespace AuroraWorld.Gameplay.World.Geometry
{
    public static class CubeMath
    {
        public static readonly Vector3Int[] Directions =
        {
            new(0, 1, -1), // верх-право 
            new(-1, 1, 0), // верх-лево 
            new(-1, 0, 1), // лево 
            new(0, -1, 1), // низ-лево
            new(1, -1, 0), // низ-право
            new(1, 0, -1), // право
        };

        public static Vector3Int ToCubeDirection(this DirectionType directionType) => Directions[(int)directionType];

        public static Vector3Int Neighbor(this Vector3Int cube, Vector3Int cubeDir) => cube + cubeDir;
        public static Vector3Int Neighbor(this Vector3Int cube, DirectionType dir) => cube + dir.ToCubeDirection();

        public static Vector3Int[] Neighbors(this Vector3Int cube)
        {
            return new[]
            {
                cube + Directions[0], cube + Directions[1], cube + Directions[2], 
                cube + Directions[3], cube + Directions[4], cube + Directions[5],
            };
        }

        public static int Distance(Vector3Int a, Vector3Int b) =>
            (Abs(a.x - b.x) + Abs(a.y - b.y) + Abs(a.z - b.z)) / 2;

        public static int DoubleWidthDistance(Vector3Int a, Vector3Int b) =>
            HexMath.DoubleWidthDistance(a.ToHex(), b.ToHex());

        public static int DoubleHeightDistance(Vector3Int a, Vector3Int b) =>
            HexMath.DoubleHeightDistance(a.ToHex(), b.ToHex());

        public static Vector3 CubeLerp(Vector3Int a, Vector3Int b, float t)
        {
            return new Vector3(
                Lerp(a.x, b.x, t),
                Lerp(a.y, b.y, t),
                Lerp(a.z, b.z, t)
            );
        }

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

        public static Vector3Int[] Line(Vector3Int a, Vector3Int b)
        {
            var distance = Distance(a, b);
            var pathPoints = new Vector3Int[distance + 1];
            for (int i = 0; i <= distance; i++)
            {
                pathPoints[i] = Round(CubeLerp(a, b, 1.0f / distance * i));
            }

            return pathPoints;
        }

        public static Vector3Int[] Range(Vector3Int center, int n)
        {
            var results = new Vector3Int[3 * (n + 1) * (n + 1) - 3 * (n + 1) + 1];
            int i = 0;
            for (int q = -n; q <= n; q++)
            {
                for (int r = Max(-n, -q - n); r <= Min(+n, -q + n); r++)
                {
                    var s = -q - r;
                    results[i] = new Vector3Int(q, r, s) + center;
                    i++;
                }
            }

            return results;
        }

        public static Vector3Int[] Ring(Vector3Int center, int radius)
        {
            var results = new Vector3Int[6 * radius];
            var cube = center + Directions[5] * radius;
            var index = 0;
            for (var i = 0; i < 6; i++)
            {
                for (var j = 0; j < radius; j++)
                {
                    results[index] = cube;
                    cube = cube.Neighbor((DirectionType)i);
                    index++;
                }
            }

            return results.ToArray();
        }

        public static Vector3Int[] IntersectingRanges(Vector3Int[] range1, Vector3Int[] range2) =>
            range1.Where(range2.Contains).ToArray();
    }
}