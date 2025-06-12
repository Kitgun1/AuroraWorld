using System.Collections.Generic;
using AuroraWorld.Gameplay.World.Geometry;
using UnityEngine;

namespace AuroraWorld.Gameplay.World.Data
{
    public class HexEntityMesh
    {
        public Vector3[] Vertices;
        public Color[] Colors;
        public int[] Triangles;
        public Dictionary<string, Vector2[]> UVs = new();
        public Edge[] InnerEdges;
        public Edge[] Edges;
        public Edge[] OuterEdges;
    }
}