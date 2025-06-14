using System.Collections.Generic;
using System.Linq;
using AuroraWorld.Gameplay.World.Data;
using UnityEngine;
using static UnityEngine.Mathf;
using Vector3 = UnityEngine.Vector3;

namespace AuroraWorld.Gameplay.World.Geometry
{
    public static class GeometryHexagon
    {
        public const float Y_MULTIPLIER = 4f;
        public const float SIZE_Y = 1f;
        public const float HEIGHT = SIZE_Y / 2;
        public const float SIZE_X = 0.8660254038f;
        public const float WIDTH = HEIGHT * SIZE_X;
        public const float ELEVATION_STEP = 0.02f;

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

        public static Vector3 HexToWorld(this Vector2Int hex, float y = 0f)
        {
            var offsetX = hex.y % 2 == 0 ? 0 : WIDTH;
            return new Vector3(hex.x * SIZE_X - offsetX, y, hex.y * SIZE_Y * 0.75f);
        }

        public static Vector3 CubeToWorld(this Vector3Int cube, float y = 0f) => cube.ToHex().HexToWorld(y);

        public static Vector2Int WorldToHex(this Vector3 world)
        {
            var hexY = RoundToInt(world.z / (SIZE_Y * 0.75f));
            var offsetX = hexY % 2 == 0 ? 0 : WIDTH;
            var hexX = RoundToInt((world.x + offsetX) / SIZE_X);
            return new Vector2Int(hexX, hexY);
        }

        public static Vector3Int WorldToCube(this Vector3 world) => world.WorldToHex().ToCube();

        #endregion

        #region Geometry

        public static void CalculateEdges(HexagonMesh hexagonMesh, Vector3Int cube, Terrain terrain)
        {
            // Грани
            hexagonMesh.InnerEdges = new Edge[6];
            hexagonMesh.Edges = new Edge[6];
            hexagonMesh.OuterEdges = new Edge[6];

            for (var i = 1; i <= 6; i++)
            {
                var direction = (DirectionType)(i - 1);
                var neighborPosition = cube.Neighbor(direction);
                var neighborElevation = terrain.GetHexagonInfo(neighborPosition).Elevation.Value;
                var neighborVertices = GetVertices(neighborPosition.CubeToWorld(neighborElevation));

                // Внутренняя грань
                var innerP1 = hexagonMesh.Vertices[i];
                var innerP2 = hexagonMesh.Vertices[i == 6 ? 1 : i + 1];
                hexagonMesh.InnerEdges[i - 1] = new Edge(cube, innerP1, innerP2, direction);

                // Внешняя грань
                var outerP1 = neighborVertices[i + 1 > 3 ? i + 1 - 3 : i + 1 + 3];
                var outerP2 = neighborVertices[i > 3 ? i - 3 : i + 3];
                hexagonMesh.OuterEdges[i - 1] = new Edge(cube, outerP1, outerP2, direction);
            }

            for (var i = 1; i <= 6; i++)
            {
                var direction = (DirectionType)(i - 1);
                // Средняя грань
                var beforeEdge = i == 1 ? 5 : i - 2;
                var afterEdge = i == 6 ? 0 : i;
                var p1 = GetCenter(hexagonMesh.InnerEdges[i - 1].P1,
                    hexagonMesh.OuterEdges[i - 1].P1,
                    hexagonMesh.OuterEdges[beforeEdge].P2);
                
                var p2 = GetCenter(hexagonMesh.InnerEdges[i - 1].P2,
                    hexagonMesh.OuterEdges[i - 1].P2,
                    hexagonMesh.OuterEdges[afterEdge].P1);

                hexagonMesh.Edges[i - 1] = new Edge(cube, p1, p2, direction);
            }

            return;

            Vector3 GetCenter(Vector3 a, Vector3 b, Vector3 c)
            {
                return (a + b + c) / 3f;
            }
        }

        private static Vector3 GetVertex(this Vector3 center, float i, float offset = 0.05f)
        {
            var angle = Deg2Rad * (60 * i + 30);
            return new Vector3(
                center.x + Cos(angle) * (HEIGHT - offset),
                center.y + center.y * Y_MULTIPLIER,
                center.z + Sin(angle) * (HEIGHT - offset));
        }

        private static Vector3[] GetVertices(Vector3 worldCenter)
        {
            var vertices = new Vector3[7];
            vertices[0] = worldCenter + Vector3.up * (worldCenter.y * Y_MULTIPLIER);
            for (var i = 0; i < 6; i++) vertices[i + 1] = worldCenter.GetVertex(i);

            return vertices;
        }

        #endregion

        public static HexagonMesh InstanceUpSideMesh(Vector3Int cube, HexagonWorldInfoProxy info)
        {
            var hexMesh = new HexagonMesh();

            var worldCenter = cube.CubeToWorld(info.Elevation.Value);

            // Вершины лицевой стороны
            var vertexes = GetVertices(worldCenter);
            hexMesh.Vertices = vertexes;

            // Помечаем вершины, которые в шейдере будут выделены линиями
            hexMesh.UVs["uv2"] = GetEdgesUV();

            // Треугольники и цвета лицевой стороны
            hexMesh.Triangles = new int[18];
            hexMesh.Colors = new Color32[vertexes.Length];
            hexMesh.Colors[0] = info.GetBiomeColor(cube.GetHashCode());
            for (var i = 0; i < 6; i++)
            {
                hexMesh.Triangles[i * 3 + 0] = i + 2 > 6 ? 1 : i + 2;
                hexMesh.Triangles[i * 3 + 1] = i + 1;
                hexMesh.Triangles[i * 3 + 2] = 0;

                hexMesh.Colors[i + 1] = info.GetBiomeColor(cube.GetHashCode());
            }

            return hexMesh;

            Vector2[] GetEdgesUV()
            {
                var uv2 = new Vector2[7];
                uv2[0] = Vector2.zero;
                for (var i = 1; i <= 6; i++) uv2[i] = Vector2.one;
                return uv2;
            }
        }

        public static void InstanceBorders(Vector3Int position, HexagonMesh mesh, Terrain terrain)
        {
            var baseColor = mesh.Colors[0];
            var addedVertices = new List<Vector3>();
            var addedUVs2 = new List<Vector2>();
            var addedColors = new List<Color32>();
            var addedTriangles = new List<int>();

            // Мосты: Строим только через 1
            for (var i = 0; i < mesh.InnerEdges.Length; i += 2)
            {
                var lastVertexIndex = mesh.Vertices.Length + addedVertices.Count;
                var innerEdge = mesh.InnerEdges[i];
                var outerEdge = mesh.OuterEdges[i];

                addedVertices.Add(innerEdge.P1);
                addedVertices.Add(innerEdge.P2);
                addedVertices.Add(outerEdge.P1);
                addedVertices.Add(outerEdge.P2);

                addedUVs2.AddRange(new[] { Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero });

                var neighborPosition = position.Neighbor(innerEdge.Direction);
                var gammaSeed = neighborPosition.GetHashCode();
                var neighborInfo = terrain.GetHexagonInfo(neighborPosition);
                var neighborVertexColor = neighborInfo.GetBiomeColor(gammaSeed);
                var neighborHidden = neighborInfo.FogOfWarState.Value == FogOfWarState.Hidden;
                var selfHidden = baseColor.a == 0;
                var selfColor = neighborHidden ? neighborVertexColor : baseColor;
                var neighborColor = selfHidden ? baseColor : neighborVertexColor;
                addedColors.Add(selfColor);
                addedColors.Add(selfColor);
                addedColors.Add(neighborColor);
                addedColors.Add(neighborColor);

                addedTriangles.AddRange(new[] { lastVertexIndex, lastVertexIndex + 1, lastVertexIndex + 3 });
                addedTriangles.AddRange(new[] { lastVertexIndex, lastVertexIndex + 3, lastVertexIndex + 2 });
            }

            // Углы: Строим только через 1
            for (var i = 0; i < mesh.InnerEdges.Length / 2; i += 1)
            {
                var lastVertexIndex = mesh.Vertices.Length + addedVertices.Count;
                var innerEdge = mesh.InnerEdges[i];
                var outerEdge = mesh.OuterEdges[i];
                var beforeOuterEdge = mesh.OuterEdges[i == 0 ? 5 : i - 1];


                addedVertices.Add(innerEdge.P1);
                addedVertices.Add(outerEdge.P1);
                addedVertices.Add(beforeOuterEdge.P2);

                addedUVs2.AddRange(new[] { Vector2.zero, Vector2.zero, Vector2.zero });

                var neighbor1Position = position.Neighbor(outerEdge.Direction);
                var neighbor1Info = terrain.GetHexagonInfo(neighbor1Position);
                var gammaSeed = neighbor1Position.GetHashCode();
                var neighbor1VertexColor = neighbor1Info.GetBiomeColor(gammaSeed);
                var neighbor1Hidden = neighbor1Info.FogOfWarState.Value == FogOfWarState.Hidden;

                var neighbor2Position = position.Neighbor(beforeOuterEdge.Direction);
                var neighbor2Info = terrain.GetHexagonInfo(neighbor2Position);
                gammaSeed = neighbor2Position.GetHashCode();
                var neighbor2VertexColor = neighbor2Info.GetBiomeColor(gammaSeed);
                var neighbor2Hidden = neighbor2Info.FogOfWarState.Value == FogOfWarState.Hidden;

                var selfHidden = baseColor.a == 0;
                var selfColor = neighbor1Hidden
                    ? neighbor1VertexColor
                    : neighbor2Hidden
                        ? neighbor2VertexColor
                        : baseColor;
                var neighbor1Color = selfHidden
                    ? baseColor
                    : neighbor2Hidden
                        ? neighbor2VertexColor
                        : neighbor1VertexColor;
                var neighbor2Color = selfHidden
                    ? baseColor
                    : neighbor1Hidden
                        ? neighbor1VertexColor
                        : neighbor2VertexColor;

                addedColors.Add(selfColor);
                addedColors.Add(neighbor1Color);
                addedColors.Add(neighbor2Color);

                addedTriangles.AddRange(new[] { lastVertexIndex, lastVertexIndex + 1, lastVertexIndex + 2 });
            }

            if (addedVertices.Count == 0) return;

            var vertices = mesh.Vertices.ToList();
            vertices.AddRange(addedVertices);
            mesh.Vertices = vertices.ToArray();

            var uv2 = mesh.UVs["uv2"].ToList();
            uv2.AddRange(addedUVs2);
            mesh.UVs["uv2"] = uv2.ToArray();

            var colors = mesh.Colors.ToList();
            colors.AddRange(addedColors);
            mesh.Colors = colors.ToArray();

            var triangles = mesh.Triangles.ToList();
            triangles.AddRange(addedTriangles);
            mesh.Triangles = triangles.ToArray();
        }
    }
}