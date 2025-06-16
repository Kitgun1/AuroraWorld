using UnityEngine;

namespace AuroraWorld.Gameplay.Player.InputData
{
    public struct ClickData
    {
        public readonly int MouseButton;
        public Vector3 ScreenPosition;
        public Vector3 WorldPosition;
        public Modifiers Modifiers;

        public ClickData(int mouseButton, Vector3 screen, Vector3 world, Modifiers modifiers)
        {
            MouseButton = mouseButton;
            ScreenPosition = screen;
            WorldPosition = world;
            Modifiers = modifiers;
        }
    }
}