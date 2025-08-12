using UnityEngine;

namespace AuroraWorld.Gameplay.World.Geometry
{
    public class Edge
    {
        public Vector3Int CubePosition;
        public Vector3 P1;
        public Vector3 P2;
        public DirectionType Direction;

        public Edge(Vector3Int cubePosition, Vector3 p1, Vector3 p2, DirectionType direction)
        {
            CubePosition = cubePosition;
            P1 = p1;
            P2 = p2;
            Direction = direction;
        }
    }
}