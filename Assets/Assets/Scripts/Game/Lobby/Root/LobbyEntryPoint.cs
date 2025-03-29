using System;
using AuroraWorld.Game.Lobby.Root.View;
using DI;
using UnityEngine;

namespace AuroraWorld.Game.Lobby.Root
{
    public class LobbyEntryPoint : MonoBehaviour
    {
        [SerializeField] private UILobbyRootBinder _sceneUIRootPrefab;

        private DIContainer _lobbyContainer;

        public event Action GoToGameplaySceneRequested;

        public void Run(DIContainer parentContainer)
        {
            _lobbyContainer = new DIContainer(parentContainer);
            _lobbyContainer.RegisterSingleton(UI_CAMERA_CONFIG, UICameraConfigFactory);
            _lobbyContainer.RegisterSingleton(UI_CAMERA, UICameraFactory);

            var uiScene = Instantiate(_sceneUIRootPrefab);
            var uiRootView = _lobbyContainer.Resolve<UIRootView>();
            uiRootView.AttachSceneUI(uiScene.gameObject);
            uiScene.GoToGameplayButtonClicked += () => GoToGameplaySceneRequested?.Invoke();
        }

        #region Factories

        private const string UI_CAMERA_CONFIG = nameof(UI_CAMERA_CONFIG);
        private const string UI_CAMERA = nameof(UI_CAMERA);

        private static Camera UICameraFactory(DIContainer c) => new(c.Resolve<CameraConfiguration>(UI_CAMERA_CONFIG));

        private static CameraConfiguration UICameraConfigFactory(DIContainer _)
            => new(null, (0, 10), CameraClearFlags.Nothing, true, 1 << LayerMask.NameToLayer("UI"));

        #endregion
    }
}