using System.Collections.Generic;
using System.Linq;
using AuroraWorld.Gameplay.World.Data;
using UnityEngine;
using static UnityEngine.Mathf;
using Vector3 = UnityEngine.Vector3;

namespace AuroraWorld.Gameplay.World.Geometry
{
    public static class Hexagon
    {
        public const float Y_MULTIPLIER = 4f;
        public const float SIZE_Y = 1f;
        public const float HEIGHT = SIZE_Y / 2;
        public const float SIZE_X = 0.8660254038f;
        public const float WIDTH = HEIGHT * SIZE_X;

        #region Convertings

        public static Vector2Int ToHex(this Vector3Int cube)
        {
            var col = cube.x + (cube.y + (cube.y & 1)) / 2;
            var row = cube.y;
            return new Vector2Int(col, row);
        }

        public static Vector3Int ToCube(this Vector2Int hex)
        {
            var x = hex.x - (hex.y + (hex.y & 1)) / 2;
            var y = hex.y;
            return new Vector3Int(x, y, -x - y);
        }

        public static Vector3 CubeToWorld(this Vector3Int cube, float y = 0f) => cube.ToHex().HexToWorld(y);

        public static Vector3 HexToWorld(this Vector2Int hex, float y = 0f)
        {
            var offsetX = hex.y % 2 == 0 ? 0 : WIDTH;
            return new Vector3(hex.x * SIZE_X - offsetX, y, hex.y * SIZE_Y * 0.75f);
        }

        public static Vector2Int WorldToHex(this Vector3 world)
        {
            int hexY = RoundToInt(world.z / (SIZE_Y * 0.75f));
            float offsetX = hexY % 2 == 0 ? 0 : WIDTH;
            int hexX = RoundToInt((world.x + offsetX) / SIZE_X);
            return new Vector2Int(hexX, hexY);
        }

        #endregion

        public static Vector3 GetVertex(this Vector3 center, float i, float offset = 0.05f)
        {
            var angle = Deg2Rad * (60 * i + 30);
            return new Vector3(
                center.x + Cos(angle) * (HEIGHT - offset),
                center.y + center.y * Y_MULTIPLIER,
                center.z + Sin(angle) * (HEIGHT - offset));
        }

        public static HexEntityMesh CalculateUpSideAndEdges(GeoConfiguration config, Vector3Int position,
            HexWorldInfoProxy info, HexEntityProxy[] neighbors)
        {
            var meshData = new HexEntityMesh();

            var worldPos = position.CubeToWorld(info.Elevation.Value);

            // Вершины лицевой стороны
            var vertexes = GetVertices(worldPos);
            meshData.Vertices = vertexes;

            // Треугольники лицевой стороны
            meshData.Triangles = new int[18];
            for (int i = 0; i < 6; i++)
            {
                meshData.Triangles[i * 3 + 0] = i + 2 > 6 ? 1 : i + 2;
                meshData.Triangles[i * 3 + 1] = i + 1;
                meshData.Triangles[i * 3 + 2] = 0;
            }

            // Цвета для вершин лицевой стороны
            var color = config.GetColor(position.ToHex());
            meshData.Colors = new Color[vertexes.Length];
            for (var i = 0; i < meshData.Colors.Length; i++) meshData.Colors[i] = color;

            // Грани
            meshData.InnerEdges = new Edge[6];
            meshData.Edges = new Edge[6];
            meshData.OuterEdges = new Edge[6];
            for (int i = 1; i <= 6; i++)
            {
                var direction = (DirectionType)(i - 1);
                var neighborPosition = position.Neighbor(direction);
                var neighbor = neighbors.FirstOrDefault(n => n?.Position == neighborPosition);
                var neighborElevation = neighbor != null
                    ? neighbor.WorldInfoProxy.Elevation.Value
                    : config.GetHexagonInfo(neighborPosition.ToHex()).Elevation.Value;
                var neighborVertices = GetVertices(neighborPosition.CubeToWorld(neighborElevation));
                // Внутренняя грань
                var p1 = meshData.Vertices[i];
                var p2 = meshData.Vertices[i == 6 ? 1 : i + 1];
                meshData.InnerEdges[i - 1] = new Edge(position, p1, p2, direction);

                // Внешняя грань
                p1 = neighborVertices[i + 1 > 3 ? i + 1 - 3 : i + 1 + 3];
                p2 = neighborVertices[i > 3 ? i - 3 : i + 3];
                meshData.OuterEdges[i - 1] = new Edge(position, p1, p2, direction);
            }

            for (int i = 1; i <= 6; i++)
            {
                var direction = (DirectionType)(i - 1);
                // Грань
                var beforeEdge = i == 1 ? 5 : i - 2;
                var afterEdge = i == 6 ? 0 : i;
                var p1 = GetCenter(meshData.InnerEdges[i - 1].P1, meshData.OuterEdges[i - 1].P1, meshData.OuterEdges[beforeEdge].P2);
                var p2 = GetCenter(meshData.InnerEdges[i - 1].P2, meshData.OuterEdges[i - 1].P2, meshData.OuterEdges[afterEdge].P1);

                meshData.Edges[i - 1] = new Edge(position, p1, p2, direction);
            }

            return meshData;

            Vector3[] GetVertices(Vector3 center)
            {
                var vertices = new Vector3[7];
                vertices[0] = center + Vector3.up * (center.y * Y_MULTIPLIER);
                for (var i = 0; i < 6; i++)
                {
                    vertices[i + 1] = center.GetVertex(i);
                }

                return vertices;
            }

            Vector3 GetCenter(Vector3 a, Vector3 b, Vector3 c)
            {
                return (a + b + c) / 3f;
            }
        }

        public static HexEntityMesh CalculateHexagonBorders(GeoConfiguration config, Vector3Int position,
            HexEntityMesh mesh, HexWorldInfoProxy[] neighbors)
        {
            var addedVertices = new List<Vector3>();
            var addedColors = new List<Color>();
            var addedTriangles = new List<int>();

            for (int i = 0; i < mesh.InnerEdges.Length; i++)
            {
                // Мосты: строим, если сосед уже есть
                if (neighbors[i] is not null)
                {
                    var lastVertexIndex = mesh.Vertices.Length + addedVertices.Count;
                    var innerEdge = mesh.InnerEdges[i];
                    var outerEdge = mesh.OuterEdges[i];

                    addedVertices.Add(innerEdge.P1);
                    addedVertices.Add(innerEdge.P2);
                    addedVertices.Add(outerEdge.P1);
                    addedVertices.Add(outerEdge.P2);

                    addedColors.Add(config.GetColor(position.ToHex()));
                    addedColors.Add(config.GetColor(position.ToHex()));
                    addedColors.Add(config.GetColor(position.Neighbor(outerEdge.Direction).ToHex()));
                    addedColors.Add(config.GetColor(position.Neighbor(outerEdge.Direction).ToHex()));

                    addedTriangles.AddRange(new[] { lastVertexIndex, lastVertexIndex + 1, lastVertexIndex + 3 });
                    addedTriangles.AddRange(new[] { lastVertexIndex, lastVertexIndex + 3, lastVertexIndex + 2 });
                }
            }

            for (int i = 0; i < mesh.InnerEdges.Length; i++)
            {
                // Углы: строим, если 2 соседа уже есть
                if (neighbors[i] is not null && neighbors[i == 0 ? 5 : i - 1] is not null)
                {
                    var lastVertexIndex = mesh.Vertices.Length + addedVertices.Count;
                    var innerEdge = mesh.InnerEdges[i];
                    var outerEdge = mesh.OuterEdges[i];
                    var beforeOuterEdge = mesh.OuterEdges[i == 0 ? 5 : i - 1];

                    addedVertices.Add(innerEdge.P1);
                    addedVertices.Add(outerEdge.P1);
                    addedVertices.Add(beforeOuterEdge.P2);


                    addedColors.Add(config.GetColor(position.ToHex()));
                    addedColors.Add(config.GetColor(position.Neighbor(outerEdge.Direction).ToHex()));
                    addedColors.Add(config.GetColor(position.Neighbor(beforeOuterEdge.Direction).ToHex()));

                    addedTriangles.AddRange(new[] { lastVertexIndex, lastVertexIndex + 1, lastVertexIndex + 2 });
                }
            }

            var vertices = mesh.Vertices.ToList();
            vertices.AddRange(addedVertices);
            mesh.Vertices = vertices.ToArray();

            var colors = mesh.Colors.ToList();
            colors.AddRange(addedColors);
            mesh.Colors = colors.ToArray();

            var triangles = mesh.Triangles.ToList();
            triangles.AddRange(addedTriangles);
            mesh.Triangles = triangles.ToArray();

            return mesh;
        }
    }
}