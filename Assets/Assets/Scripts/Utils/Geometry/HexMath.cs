using System.Linq;
using UnityEngine;
using static UnityEngine.Mathf;

namespace AuroraWorld.Gameplay.World.Geometry
{
    public static class HexMath
    {
        public static readonly Vector2Int[] Directions =
        {
            CubeMath.Directions[0].ToHex(), // верх-право
            CubeMath.Directions[1].ToHex(), // верх-лево
            CubeMath.Directions[2].ToHex(), // лево
            CubeMath.Directions[3].ToHex(), // низ-лево
            CubeMath.Directions[4].ToHex(), // низ-право
            CubeMath.Directions[5].ToHex(), // право
        };

        public static Vector2Int ToHexDirection(this DirectionType directionType) => Directions[(int)directionType];
        public static Vector2Int Neighbor(this Vector2Int hex, Vector2Int dir) => hex + dir;
        public static Vector2Int Neighbor(this Vector2Int hex, DirectionType dir) => hex + dir.ToHexDirection();

        public static int Distance(Vector2Int a, Vector2Int b) =>
            CubeMath.Distance(a.ToCube(), b.ToCube());

        public static int DoubleWidthDistance(Vector2Int a, Vector2Int b)
        {
            var dCol = Abs(a.x - b.x);
            var dRow = Abs(a.y - b.y);
            return dRow + Max(0, (dCol - dRow) / 2);
        }

        public static int DoubleHeightDistance(Vector2Int a, Vector2Int b)
        {
            var dCol = Abs(a.x - b.x);
            var dRow = Abs(a.y - b.y);
            return dCol + Max(0, (dRow - dCol) / 2);
        }

        public static Vector2Int[] Line(Vector2Int a, Vector2Int b) =>
            CubeMath.Line(a.ToCube(), b.ToCube()).Select(cube => cube.ToHex()).ToArray();

        public static Vector2Int[] Range(Vector2Int center, int n) =>
            CubeMath.Range(center.ToCube(), n).Select(cube => cube.ToHex()).ToArray();


        public static Vector2Int[] IntersectingRanges(Vector2Int[] range1, Vector2Int[] range2) =>
            range1.Where(range2.Contains).ToArray();
    }
}