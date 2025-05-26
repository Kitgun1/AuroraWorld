using System.Collections.Generic;
using System.Linq;
using AuroraWorld.Gameplay.World.Geometry;
using UnityEngine;

namespace Assets.Utils.Coroutine
{
    public class ThickLineMesh
    {
        private Vector3[] _points { get; }
        private float _thickness { get; }
        public Mesh Mesh { get; private set; }

        public void Construct(HexEntityProxy[] selectedHexes)
        {
            var resultEdges = new List<Edge>();
            foreach (var entityProxy in selectedHexes)
            {
                var edges = entityProxy.EntityMesh.Edges;
                foreach (var edge in edges)
                {
                    var edgeNeighborPosition = edge.CubePosition.Neighbor(edge.Direction);
                    if (selectedHexes.Any(e => e.Position == edgeNeighborPosition)) continue;
                    resultEdges.Add(edge);
                }
            }
        }

        private Mesh GenerateMesh()
        {
            var mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            for (int i = 0; i < _points.Length; i++)
            {
                Vector3 point = _points[i];
                Vector3 prevPoint = _points[(i - 1 + _points.Length) % _points.Length];
                Vector3 nextPoint = _points[(i + 1) % _points.Length];
                Vector3 prevDirection = (point - prevPoint).normalized;
                Vector3 nextDirection = (nextPoint - point).normalized;
                Vector3 avgDirection = (prevDirection + nextDirection).normalized;
                Vector3 normal = Vector3.Cross(avgDirection, Vector3.up).normalized;

                vertices.Add(point + normal * _thickness / 2);
                vertices.Add(point - normal * _thickness / 2);
            }

            for (int i = 0; i < _points.Length; i++)
            {
                int v1 = i * 2;
                int v2 = v1 + 1;
                int v3 = ((i + 1) % _points.Length) * 2;
                int v4 = v3 + 1;

                triangles.Add(v1);
                triangles.Add(v3);
                triangles.Add(v2);

                triangles.Add(v2);
                triangles.Add(v3);
                triangles.Add(v4);
            }

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}