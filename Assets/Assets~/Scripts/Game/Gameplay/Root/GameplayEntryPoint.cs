using AuroraWorld.App.Lobby.Root;
using AuroraWorld.Gameplay.Root.View;
using AuroraWorld.GameRoot.View;
using DI;
using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.Root
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        [SerializeField] private UIGameplayRootBinder _sceneUIRootPrefab;

        public Observable<GameplayExitParams> Run(DIContainer gameplayContainer, GameplayEnterParams enterParams)
        {
            return GameplayRegistrations.Register(gameplayContainer, enterParams)
                .SelectMany(_ =>
                {
                    var uiScene = Instantiate(_sceneUIRootPrefab);
                    var uiRootView = gameplayContainer.Resolve<UIRootView>();
                    uiRootView.AttachSceneUI(uiScene.gameObject);

                    var exitSignalSubject = new Subject<Unit>();
                    uiScene.Bind(exitSignalSubject);

                    var lobbyEnterParams = new LobbyEnterParams();
                    var gameplayExitParams = new GameplayExitParams(lobbyEnterParams);
                    var exitToLobbySceneSignal = exitSignalSubject.Select(_ => gameplayExitParams);
                    return exitToLobbySceneSignal;
                });
        }
    }
}