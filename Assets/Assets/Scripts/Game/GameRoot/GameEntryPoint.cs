using System;
using System.Collections;
using Assets.Utils.Coroutine;
using AuroraWorld.Game.Gameplay.Root;
using AuroraWorld.Game.Lobby.Root;
using DI;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AuroraWorld
{
    public class GameEntryPoint
    {
        private static GameEntryPoint _instance;

        private readonly Coroutines _coroutines;
        private readonly UIRootView _uiRoot;

        private DIContainer _rootContainer = new();
        private DIContainer _cachedSceneContainer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            _instance = new GameEntryPoint();
            _instance.RunGame();
        }

        private GameEntryPoint()
        {
            _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
            Object.DontDestroyOnLoad(_coroutines.gameObject);

            var prefabUIRoot = Resources.Load<UIRootView>("UIRoot");
            _uiRoot = Object.Instantiate(prefabUIRoot);
            Object.DontDestroyOnLoad(_uiRoot.gameObject);
            _rootContainer.RegisterInstance(_uiRoot);
            
            GameRegistrations.Register(_rootContainer);
        }

        private void RunGame()
        {
#if UNITY_EDITOR
            var sceneName = SceneManager.GetActiveScene().name;
            switch (sceneName)
            {
                case Scenes.LOBBY:
                    var lobbyEnterParams = new LobbyEnterParams("null");
                    _coroutines.StartCoroutine(LoadAndStartLobby(lobbyEnterParams));
                    return;
                case Scenes.GAMEPLAY:
                    var gameplayEnterParams = new GameplayEnterParams();
                    _coroutines.StartCoroutine(LoadAndStartGameplay(gameplayEnterParams));
                    return;
            }

            if (sceneName != Scenes.BOOT) return;
#endif
            _coroutines.StartCoroutine(LoadAndStartLobby());
        }

        private IEnumerator LoadAndStartLobby(LobbyEnterParams enterParams = null)
        {
            _uiRoot.ShowLoadingScreen();
            _cachedSceneContainer?.Dispose();

            yield return LoadScene(Scenes.BOOT);
            yield return LoadScene(Scenes.LOBBY);
            yield return null;

            var sceneEntryPoint = Object.FindObjectOfType<LobbyEntryPoint>();
            var lobbyContainer = _cachedSceneContainer = new DIContainer(_rootContainer);
            sceneEntryPoint.Run(lobbyContainer, enterParams).Subscribe(lobbyExitParams =>
            {
                var targetSceneName = lobbyExitParams.TargetSceneEnterParams.SceneName;
                IEnumerator enumerator;
                switch (targetSceneName)
                {
                    case Scenes.GAMEPLAY:
                        var gameplayEnterParams = new GameplayEnterParams();
                        enumerator = LoadAndStartGameplay(gameplayEnterParams);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _coroutines.StartCoroutine(enumerator);
            });

            _uiRoot.HideLoadingScreen();
        }

        private IEnumerator LoadAndStartGameplay(GameplayEnterParams enterParams = null)
        {
            _uiRoot.ShowLoadingScreen();
            _cachedSceneContainer?.Dispose();

            yield return LoadScene(Scenes.BOOT);
            yield return LoadScene(Scenes.GAMEPLAY);
            yield return null;

            var sceneEntryPoint = Object.FindObjectOfType<GameplayEntryPoint>();
            var gameplayContainer = _cachedSceneContainer = new DIContainer(_rootContainer);
            sceneEntryPoint.Run(gameplayContainer, enterParams).Subscribe(gameplayExitParams =>
            {
                var targetSceneName = gameplayExitParams.TargetSceneEnterParams.SceneName;
                IEnumerator enumerator;
                switch (targetSceneName)
                {
                    case Scenes.LOBBY:
                        var previousWorldName = gameplayExitParams.TargetSceneEnterParams
                            .As<LobbyEnterParams>()
                            .PreviousWorldName;
                        enumerator = LoadAndStartLobby(new LobbyEnterParams(previousWorldName));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _coroutines.StartCoroutine(enumerator);
            });

            _uiRoot.HideLoadingScreen();
        }

        private static IEnumerator LoadScene(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }
    }
}