using System.Collections;
using Assets.Utils.Coroutine;
using AuroraWorld.Game.Gameplay.Root;
using AuroraWorld.Game.Lobby.Root;
using AuroraWorld.StorageGame;
using DI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AuroraWorld
{
    public class GameEntryPoint
    {
        private static GameEntryPoint _instance;
        private static DIContainer _projectContainer;

        private readonly Coroutines _coroutines;
        private readonly UIRootView _uiRoot;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            _projectContainer = new DIContainer();
            _projectContainer.RegisterSingleton(_ => new Storage());

            _instance = new GameEntryPoint();
            _instance.RunGame();
        }

        private GameEntryPoint()
        {
            _coroutines = new GameObject("[COROUTINES]").AddComponent<Coroutines>();
            Object.DontDestroyOnLoad(_coroutines.gameObject);
            _projectContainer.RegisterInstance(_coroutines);

            var prefabUIRoot = Resources.Load<UIRootView>("UIRoot");
            _uiRoot = Object.Instantiate(prefabUIRoot);
            Object.DontDestroyOnLoad(_uiRoot.gameObject);
            _projectContainer.RegisterInstance(_uiRoot);
        }

        private void RunGame()
        {
#if UNITY_EDITOR
            var sceneName = SceneManager.GetActiveScene().name;
            switch (sceneName)
            {
                case Scenes.LOBBY:
                    _coroutines.StartCoroutine(LoadAndStartLobby());
                    return;
                case Scenes.GAMEPLAY:
                    _coroutines.StartCoroutine(LoadAndStartGameplay());
                    return;
            }

            if (sceneName != Scenes.BOOT) return;
#endif
            _coroutines.StartCoroutine(LoadAndStartLobby());
        }

        private IEnumerator LoadAndStartLobby()
        {
            _uiRoot.ShowLoadingScreen();

            yield return LoadScene(Scenes.BOOT);
            yield return LoadScene(Scenes.LOBBY);
            yield return null;

            var sceneEntryPoint = Object.FindObjectOfType<LobbyEntryPoint>();
            sceneEntryPoint.Run(_projectContainer);

            sceneEntryPoint.GoToGameplaySceneRequested += () => _coroutines.StartCoroutine(LoadAndStartGameplay());

            _uiRoot.HideLoadingScreen();
        }

        private IEnumerator LoadAndStartGameplay()
        {
            _uiRoot.ShowLoadingScreen();

            yield return LoadScene(Scenes.BOOT);
            yield return LoadScene(Scenes.GAMEPLAY);
            yield return null;

            var sceneEntryPoint = Object.FindObjectOfType<GameplayEntryPoint>();
            sceneEntryPoint.Run(_projectContainer);

            sceneEntryPoint.GoToLobbySceneRequested += () => _coroutines.StartCoroutine(LoadAndStartLobby());

            _uiRoot.HideLoadingScreen();
        }

        private IEnumerator LoadScene(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }
    }
}