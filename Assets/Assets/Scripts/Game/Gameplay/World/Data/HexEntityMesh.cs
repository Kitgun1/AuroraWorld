using AuroraWorld.Gameplay.World.Geometry;
using UnityEngine;

namespace AuroraWorld.Gameplay.World.Data
{
    public class HexEntityMesh
    {
        public Vector3[] Vertices;
        public Color[] Colors;
        public int[] Triangles;
        public Edge[] InnerEdges;
        public Edge[] Edges;
        public Edge[] OuterEdges;
    }
}