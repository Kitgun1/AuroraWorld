using AuroraWorld.Game.Gameplay.Root.View;
using AuroraWorld.Game.Lobby.Root;
using DI;
using R3;
using UnityEngine;

namespace AuroraWorld.Game.Gameplay.Root
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        [SerializeField] private UIGameplayRootBinder _sceneUIRootPrefab;

        public Observable<GameplayExitParams> Run(DIContainer gameplayContainer, GameplayEnterParams enterParams)
        {
            GameplayRegistrations.Register(gameplayContainer, enterParams);
            
            var uiScene = Instantiate(_sceneUIRootPrefab);
            var uiRootView = gameplayContainer.Resolve<UIRootView>();
            uiRootView.AttachSceneUI(uiScene.gameObject);

            var exitSignalSubject = new Subject<Unit>();
            uiScene.Bind(exitSignalSubject);

            var lobbyEnterParams = new LobbyEnterParams($"world{Random.Range(1, 25)}");
            var gameplayExitParams = new GameplayExitParams(lobbyEnterParams);
            var exitToLobbySceneSignal = exitSignalSubject.Select(_ => gameplayExitParams);

            return exitToLobbySceneSignal;
        }
    }
}