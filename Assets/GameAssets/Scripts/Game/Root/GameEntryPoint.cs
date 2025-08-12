using System;
using System.Collections;
using AuroraWorld.Core;
using AuroraWorld.GameMenu.Root;
using AuroraWorld.Gameplay.Root;
using DI;
using R3;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace AuroraWorld.Root
{
    public class GameEntryPoint
    {
        private static GameEntryPoint _instance;

        private readonly DIContainer _rootContainer;
        private readonly Coroutines _coroutines;
        private readonly UIRoot _uiRoot;

        private DIContainer _cachedSceneContainer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
#if !UNITY_EDITOR
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
#endif

            _instance = new GameEntryPoint();
            _instance.RunGame();
        }

        private GameEntryPoint()
        {
            _rootContainer = new DIContainer();

            _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
            Object.DontDestroyOnLoad(_coroutines.gameObject);
            _rootContainer.RegisterInstance(_coroutines);

            _uiRoot = new UIRoot(_rootContainer);

            // TODO: Game registration (services, modules, etc.)
        }

        private void RunGame()
        {
            #if UNITY_EDITOR
            var currentSceneName = SceneManager.GetActiveScene().name;
            switch (currentSceneName)
            {
                case Scenes.STARTING_MENU:
                    var startingMenuEnterParams = new GameMenuEnterParams(true);
                    _coroutines.StartCoroutine(LoadAndStartStartingMenu(startingMenuEnterParams));
                    return;
                case Scenes.GAMEPLAY:
                    startingMenuEnterParams = new GameMenuEnterParams(true);
                    _coroutines.StartCoroutine(LoadAndStartStartingMenu(startingMenuEnterParams));
                    return;
            }

            if (currentSceneName != Scenes.BOOT) return;
            #endif

            _coroutines.StartCoroutine(LoadAndStartStartingMenu(new GameMenuEnterParams(true)));
        }

        private IEnumerator LoadAndStartStartingMenu(GameMenuEnterParams enterParams)
        {
            _uiRoot.ShowLoadingScreen();
            _cachedSceneContainer?.Dispose();

            yield return LoadScene(Scenes.BOOT);
            yield return LoadScene(Scenes.STARTING_MENU);
            yield return null;

            var sceneEntryPoint = Object.FindFirstObjectByType<GameMenuEntryPoint>();
            var sceneContainer = _cachedSceneContainer = new DIContainer(_rootContainer);
            sceneEntryPoint.Run(sceneContainer, enterParams).Subscribe(exitParams =>
            {
                // Exit scene logic:
                Debug.Log(exitParams.TargetSceneEnterParams.As<GameplayEnterParams>().WorldName);
                Debug.Log(exitParams.TargetSceneEnterParams.As<GameplayEnterParams>().SeedHash);
                var targetSceneName = exitParams.TargetSceneEnterParams.SceneName;
                IEnumerator enumerator = null;
                switch (targetSceneName)
                {
                    case Scenes.STARTING_MENU:
                        enumerator = LoadAndStartStartingMenu(exitParams.TargetSceneEnterParams.As<GameMenuEnterParams>());
                        break;
                    case Scenes.GAMEPLAY:
                        enumerator = LoadAndStartGameplay(exitParams.TargetSceneEnterParams.As<GameplayEnterParams>());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                _coroutines.StartCoroutine(enumerator);
            });

            _uiRoot.HideLoadingScreen();
        }
        private IEnumerator LoadAndStartGameplay(GameplayEnterParams enterParams)
        {
            _uiRoot.ShowLoadingScreen();
            _cachedSceneContainer?.Dispose();

            yield return LoadScene(Scenes.BOOT);
            yield return LoadScene(Scenes.GAMEPLAY);
            yield return null;

            var sceneEntryPoint = Object.FindFirstObjectByType<GameplayEntryPoint>();
            var sceneContainer = _cachedSceneContainer = new DIContainer(_rootContainer);
            sceneEntryPoint.Run(sceneContainer, enterParams).Subscribe(exitParams =>
            {
                // Exit scene logic:
                _coroutines.StartCoroutine(LoadAndStartStartingMenu(new GameMenuEnterParams(false)));
            });

            _uiRoot.HideLoadingScreen();
        }
        private static IEnumerator LoadScene(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }
    }
}