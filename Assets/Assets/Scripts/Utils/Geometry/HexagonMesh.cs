using System.Collections.Generic;
using UnityEngine;

namespace AuroraWorld.Gameplay.World.Geometry
{
    public class HexagonMesh
    {
        public Vector3[] Vertices;
        public Color32[] Colors;
        public int[] Triangles;
        public Dictionary<string, Vector2[]> UVs = new();
        public Edge[] InnerEdges;
        public Edge[] Edges;
        public Edge[] OuterEdges;
    }
}