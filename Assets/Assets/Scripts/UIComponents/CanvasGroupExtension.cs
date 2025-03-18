using UnityEngine;

namespace AuroraWorld.UIComponents
{
    public static class CanvasGroupExtension
    {
        public static bool IsActive(this CanvasGroup canvasGroup)
        {
            return canvasGroup.alpha >= 0.6f ? true : false;
        }

        public static void Active(this CanvasGroup canvasGroup, bool active)
        {
            canvasGroup.alpha = active ? 1F : 0F;
            canvasGroup.interactable = active;
            canvasGroup.blocksRaycasts = active;
        }
    }
}