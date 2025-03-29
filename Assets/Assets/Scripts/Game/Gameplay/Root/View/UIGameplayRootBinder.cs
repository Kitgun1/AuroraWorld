using System;
using UnityEngine;

namespace AuroraWorld.Game.Gameplay.Root.View
{
    public class UIGameplayRootBinder : MonoBehaviour
    {
        public event Action GoToLobbyButtonClicked;

        public void HandleGoToLobbyButtonClick()
        {
            GoToLobbyButtonClicked?.Invoke();
        }
    }
}