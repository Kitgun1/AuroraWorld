using System;
using AuroraWorld.Game.Gameplay.Root.View;
using DI;
using UnityEngine;

namespace AuroraWorld.Game.Gameplay.Root
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        [SerializeField] private UIGameplayRootBinder _sceneUIRootPrefab;

        public event Action GoToLobbySceneRequested;

        public void Run(DIContainer parentContainer)
        {
            var gameplayContainer = new DIContainer(parentContainer);

            var uiScene = Instantiate(_sceneUIRootPrefab);
            var uiRootView = gameplayContainer.Resolve<UIRootView>();
            uiRootView.AttachSceneUI(uiScene.gameObject);
            uiScene.GoToLobbyButtonClicked += () => GoToLobbySceneRequested?.Invoke();
        }
    }
}