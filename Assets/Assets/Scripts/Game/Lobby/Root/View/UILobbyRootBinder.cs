using System;
using UnityEngine;

namespace AuroraWorld.Game.Lobby.Root.View
{
    public class UILobbyRootBinder : MonoBehaviour
    {
        public event Action GoToGameplayButtonClicked;

        public void HandleGoToGameplayButtonClick()
        {
            GoToGameplayButtonClicked?.Invoke();
        }
    }
}