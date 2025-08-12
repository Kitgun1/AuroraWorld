using UnityEngine;

namespace AuroraWorld.Gameplay.Player.InputData
{
    public struct MouseMoveData
    {
        public Vector3 ScreenPosition;
        public Vector3 WorldPosition;
        public bool IsPointerOverUI;
    }
}