using UnityEngine;

namespace AuroraWorld.Gameplay.Player.InputData
{
    public struct ClickData
    {
        public readonly int MouseButton;
        public Vector3 ScreenPosition;
        public Vector3 WorldPosition;
        public bool HasCtrl;
        public bool HasShift;
        public bool HasAlt;

        public ClickData(int mouseButton, Vector3 screen, Vector3 world, bool ctrl, bool shift, bool alt)
        {
            MouseButton = mouseButton;
            ScreenPosition = screen;
            WorldPosition = world;
            HasCtrl = ctrl;
            HasShift = shift;
            HasAlt = alt;
        }
    }
}