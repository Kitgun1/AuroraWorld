using UnityEngine;

namespace AuroraWorld.Gameplay.World.Geometry
{
    public static class ChunkConverters
    {
        public const int CHUNK_RADIUS = 8;

        public static Vector3Int ChunkCenter(Vector3Int chunk, int r = CHUNK_RADIUS)
        {
            return new Vector3Int((r + 1) * chunk.x - r * chunk.z,
                (r + 1) * chunk.y - r * chunk.x,
                (r + 1) * chunk.z - r * chunk.y);
        }

        public static Vector3Int CubeToChunk(Vector3Int cube, int r = CHUNK_RADIUS)
        {
            var localCube = CubeToLocalCube(cube, r);
            var i = Div(1 + localCube.x - localCube.y, 3);
            var j = Div(1 + localCube.y - localCube.z, 3);
            var k = Div(1 + localCube.z - localCube.x, 3);
            return new Vector3Int(i, j, k);
        }

        public static Vector3Int CubeToLocalCube(Vector3Int cube, int r = CHUNK_RADIUS)
        {
            var shift = 3 * r + 2;
            var area = 3 * r * r + 3 * r + 1;
            var xh = Div(cube.y + shift * cube.x, area);
            var yh = Div(cube.z + shift * cube.y, area);
            var zh = Div(cube.x + shift * cube.z, area);
            return new Vector3Int(xh, yh, zh);
        }

        private static int Div(int x, int y) => Mathf.FloorToInt(x / (float)y);
    }
}