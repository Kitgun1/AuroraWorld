using UnityEngine;

namespace AuroraWorld.Core.Mathematics.Base
{
    public class Edge
    {
        /// <summary> Position in cube space </summary>
        public readonly Vector3Int Position;
        public readonly Vector3 P1;
        public readonly Vector3 P2;
        public readonly DirectionType Direction;

        public Edge(Vector3Int cubePosition, Vector3 p1, Vector3 p2, DirectionType direction)
        {
            Position = cubePosition;
            P1 = p1;
            P2 = p2;
            Direction = direction;
        }
    }
}