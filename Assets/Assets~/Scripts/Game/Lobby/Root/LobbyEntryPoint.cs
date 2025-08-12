using AuroraWorld.App.Lobby.Root.View;
using AuroraWorld.Gameplay.Root;
using AuroraWorld.GameRoot.View;
using DI;
using R3;
using UnityEngine;

namespace AuroraWorld.App.Lobby.Root
{
    public class LobbyEntryPoint : MonoBehaviour
    {
        [SerializeField] private UILobbyRootBinder _sceneUIRootPrefab;

        public Observable<LobbyExitParams> Run(DIContainer lobbyContainer, LobbyEnterParams enterParams)
        {
            LobbyRegistrations.Register(lobbyContainer, enterParams);
            
            // _lobbyContainer.RegisterSingleton(UI_CAMERA_CONFIG, UICameraConfigFactory);
            // _lobbyContainer.RegisterSingleton(UI_CAMERA, UICameraFactory);

            var uiScene = Instantiate(_sceneUIRootPrefab);
            var uiRootView = lobbyContainer.Resolve<UIRootView>();
            uiRootView.AttachSceneUI(uiScene.gameObject);

            var exitSceneSignalSubject = new Subject<Unit>();
            uiScene.Bind(exitSceneSignalSubject);

            // TODO: Название мира выбираем в лобби
            var gameplayEnterParams = new GameplayEnterParams("sandbox", "sandbox");
            var lobbyExitParams = new LobbyExitParams(gameplayEnterParams);
            var exitToLobbySceneSignal = exitSceneSignalSubject.Select(_ => lobbyExitParams);

            return exitToLobbySceneSignal;
        }
    }
}