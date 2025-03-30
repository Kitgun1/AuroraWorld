using AuroraWorld.Game.Gameplay.Root;
using AuroraWorld.Game.Lobby.Root.View;
using DI;
using R3;
using UnityEngine;

namespace AuroraWorld.Game.Lobby.Root
{
    public class LobbyEntryPoint : MonoBehaviour
    {
        [SerializeField] private UILobbyRootBinder _sceneUIRootPrefab;

        private DIContainer _lobbyContainer;

        public Observable<LobbyExitParams> Run(DIContainer parentContainer, LobbyEnterParams enterParams)
        {
            _lobbyContainer = new DIContainer(parentContainer);
            // _lobbyContainer.RegisterSingleton(UI_CAMERA_CONFIG, UICameraConfigFactory);
            // _lobbyContainer.RegisterSingleton(UI_CAMERA, UICameraFactory);

            var uiScene = Instantiate(_sceneUIRootPrefab);
            var uiRootView = _lobbyContainer.Resolve<UIRootView>();
            uiRootView.AttachSceneUI(uiScene.gameObject);

            Debug.Log($"previous world: '{enterParams?.PreviousWorldName}'");
            
            var exitSceneSignalSubject = new Subject<Unit>();
            uiScene.Bind(exitSceneSignalSubject);

            var gameplayEnterParams = new GameplayEnterParams();
            var lobbyExitParams = new LobbyExitParams(gameplayEnterParams);
            var exitToLobbySceneSignal = exitSceneSignalSubject.Select(_ => lobbyExitParams);
            
            return exitToLobbySceneSignal;
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