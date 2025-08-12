using UnityEngine;

namespace AuroraWorld.Gameplay.Player.InputData
{
    public struct AxesData
    {
        public float Horizontal;
        public float Vertical;
        public Modifiers Modifiers;

        public Vector2 Vector => new Vector2(Horizontal, Vertical).normalized;
    }
}