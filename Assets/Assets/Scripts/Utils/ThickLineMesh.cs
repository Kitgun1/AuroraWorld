using System.Collections.Generic;
using System.Linq;
using AuroraWorld.Gameplay.World.Geometry;
using UnityEngine;

namespace Assets.Utils.Coroutine
{
    public class ThickLineMesh
    {
        private (Edge inner, Edge center, Edge outer)?[][] _lines;

        public Mesh Mesh { get; private set; }
        public HexagonProxy[] SelectedHexagons { get; private set; } = { };

        public void AttachMesh(HexagonProxy[] selectedHexes)
        {
            SelectedHexagons = selectedHexes;
            _lines = GeneratePath(selectedHexes);
            Mesh = GenerateMesh();
        }

        private (Edge inner, Edge center, Edge outer)?[][] GeneratePath(HexagonProxy[] selectedHexes)
        {
            selectedHexes = selectedHexes.GroupBy(h => h.Position).Select(g => g.First()).ToArray();

            var resultEdges = new List<(Edge inner, Edge center, Edge outer)?>();
            var lines = new List<(Edge inner, Edge center, Edge outer)?[]>();

            foreach (var entityProxy in selectedHexes)
            {
                var edges = entityProxy.HexMesh.Edges;
                for (var i = 0; i < edges.Length; i++)
                {
                    var edge = edges[i];
                    var neighborPosition = edge.CubePosition.Neighbor(edge.Direction);
                    if (selectedHexes.Any(e => e.Position == neighborPosition)) continue;
                    resultEdges.Add(
                        (entityProxy.HexMesh.InnerEdges[i],
                            entityProxy.HexMesh.Edges[i],
                            entityProxy.HexMesh.OuterEdges[i]));
                }
            }

            var notCheckedEdge = new List<(Edge inner, Edge center, Edge outer)?>(resultEdges);
            var currentEdge = notCheckedEdge.Count != 0 ? notCheckedEdge[0] : null;

            var line = new List<(Edge inner, Edge center, Edge outer)?>();
            while (true)
            {
                line.Add(currentEdge);
                notCheckedEdge.Remove(currentEdge);
                currentEdge = notCheckedEdge.FirstOrDefault(e => e?.center.P1 == currentEdge?.center.P2);
                if (currentEdge == null)
                {
                    lines.Add(line.ToArray());
                    if (notCheckedEdge.Count == 0) break;
                    line = new List<(Edge inner, Edge center, Edge outer)?>();
                    currentEdge = notCheckedEdge[0];
                }
            }

            return lines.ToArray();
        }

        private Mesh GenerateMesh()
        {
            Mesh mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            var trianglesOffset = 0;
            foreach (var line in _lines)
            {
                if (line == null) continue;
                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] == null || line[(i + 1) % line.Length] == null) continue;
                    var edges = line[i].Value;
                    var afterEdges = line[(i + 1) % line.Length].Value;
                    var maxY = Mathf.Max(edges.inner.P1.y, edges.outer.P1.y) + 0.1f;
                    var maxEndY = Mathf.Max(afterEdges.inner.P1.y, afterEdges.outer.P1.y) + 0.1f;

                    // Верхняя грань
                    vertices.Add(new Vector3(edges.inner.P1.x, maxY, edges.inner.P1.z));
                    vertices.Add(new Vector3(edges.inner.P2.x, maxY, edges.inner.P2.z));
                    vertices.Add(new Vector3(edges.outer.P1.x, maxY, edges.outer.P1.z));
                    vertices.Add(new Vector3(edges.outer.P2.x, maxY, edges.outer.P2.z));

                    triangles.Add(trianglesOffset + 0);
                    triangles.Add(trianglesOffset + 1);
                    triangles.Add(trianglesOffset + 3);
                    triangles.Add(trianglesOffset + 0);
                    triangles.Add(trianglesOffset + 3);
                    triangles.Add(trianglesOffset + 2);

                    trianglesOffset = vertices.Count;

                    // Верхняя грань между гранями гексов
                    if (maxY < maxEndY)
                    {
                        vertices.Add(new Vector3(edges.inner.P2.x, maxY, edges.inner.P2.z));
                        vertices.Add(new Vector3(edges.outer.P2.x, maxY, edges.outer.P2.z));
                        vertices.Add(new Vector3(edges.inner.P2.x, maxEndY, edges.inner.P2.z));
                        vertices.Add(new Vector3(edges.outer.P2.x, maxEndY, edges.outer.P2.z));
                    }
                    else
                    {
                        vertices.Add(new Vector3(afterEdges.inner.P1.x, maxY, afterEdges.inner.P1.z));
                        vertices.Add(new Vector3(afterEdges.outer.P1.x, maxY, afterEdges.outer.P1.z));
                        vertices.Add(new Vector3(afterEdges.inner.P1.x, maxEndY, afterEdges.inner.P1.z));
                        vertices.Add(new Vector3(afterEdges.outer.P1.x, maxEndY, afterEdges.outer.P1.z));
                    }

                    triangles.Add(trianglesOffset + 0);
                    triangles.Add(trianglesOffset + 3);
                    triangles.Add(trianglesOffset + 1);
                    triangles.Add(trianglesOffset + 0);
                    triangles.Add(trianglesOffset + 2);
                    triangles.Add(trianglesOffset + 3);

                    trianglesOffset = vertices.Count;

                    vertices.Add(new Vector3(edges.inner.P2.x, Mathf.Max(maxEndY, maxY), edges.inner.P2.z));
                    vertices.Add(new Vector3(edges.outer.P2.x, Mathf.Max(maxEndY, maxY), edges.outer.P2.z));
                    vertices.Add(new Vector3(afterEdges.inner.P1.x, Mathf.Max(maxEndY, maxY), afterEdges.inner.P1.z));
                    vertices.Add(new Vector3(afterEdges.outer.P1.x, Mathf.Max(maxEndY, maxY), afterEdges.outer.P1.z));

                    triangles.Add(trianglesOffset + 0);
                    triangles.Add(trianglesOffset + 3);
                    triangles.Add(trianglesOffset + 1);
                    triangles.Add(trianglesOffset + 0);
                    triangles.Add(trianglesOffset + 2);
                    triangles.Add(trianglesOffset + 3);

                    trianglesOffset = vertices.Count;

                    // Внутренняя грань
                    vertices.Add(edges.inner.P1);
                    vertices.Add(edges.inner.P2);
                    vertices.Add(new Vector3(edges.inner.P1.x, maxY, edges.inner.P1.z));
                    vertices.Add(new Vector3(edges.inner.P2.x, maxY, edges.inner.P2.z));

                    triangles.Add(trianglesOffset + 0);
                    triangles.Add(trianglesOffset + 1);
                    triangles.Add(trianglesOffset + 3);
                    triangles.Add(trianglesOffset + 0);
                    triangles.Add(trianglesOffset + 3);
                    triangles.Add(trianglesOffset + 2);

                    trianglesOffset = vertices.Count;

                    // Внутренняя грань между гранями гексов
                    vertices.Add(edges.inner.P2);
                    vertices.Add(afterEdges.inner.P1);
                    vertices.Add(new Vector3(edges.inner.P2.x, Mathf.Max(maxEndY, maxY), edges.inner.P2.z));
                    vertices.Add(new Vector3(afterEdges.inner.P1.x, Mathf.Max(maxEndY, maxY), afterEdges.inner.P1.z));

                    triangles.Add(trianglesOffset + 0);
                    triangles.Add(trianglesOffset + 1);
                    triangles.Add(trianglesOffset + 3);
                    triangles.Add(trianglesOffset + 0);
                    triangles.Add(trianglesOffset + 3);
                    triangles.Add(trianglesOffset + 2);


                    trianglesOffset = vertices.Count;

                    // Внешняя грань
                    vertices.Add(new Vector3(edges.outer.P1.x, maxY, edges.outer.P1.z));
                    vertices.Add(new Vector3(edges.outer.P2.x, maxY, edges.outer.P2.z));
                    vertices.Add(edges.outer.P1);
                    vertices.Add(edges.outer.P2);

                    triangles.Add(trianglesOffset + 0);
                    triangles.Add(trianglesOffset + 1);
                    triangles.Add(trianglesOffset + 3);
                    triangles.Add(trianglesOffset + 0);
                    triangles.Add(trianglesOffset + 3);
                    triangles.Add(trianglesOffset + 2);

                    trianglesOffset = vertices.Count;

                    // Внешняя грань между гранями гексов
                    if (Mathf.Approximately(maxY, maxEndY))
                    {
                        vertices.Add(edges.outer.P2);
                        vertices.Add(afterEdges.outer.P1);
                        vertices.Add(new Vector3(edges.outer.P2.x, maxY, edges.outer.P2.z));
                        vertices.Add(new Vector3(afterEdges.outer.P1.x, maxEndY, afterEdges.outer.P1.z));

                        triangles.Add(trianglesOffset + 0);
                        triangles.Add(trianglesOffset + 3);
                        triangles.Add(trianglesOffset + 1);
                        triangles.Add(trianglesOffset + 0);
                        triangles.Add(trianglesOffset + 2);
                        triangles.Add(trianglesOffset + 3);
                    }

                    trianglesOffset = vertices.Count;
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}