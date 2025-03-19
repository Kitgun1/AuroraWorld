using AuroraWorld.StorageGame;
using DI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AuroraWorld
{
    public static class ProjectContext
    {
        private static DIContainer _projectContainer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            _projectContainer = new DIContainer();
            _projectContainer.RegisterSingleton(_ => new Storage());

            var operation = SceneManager.LoadSceneAsync("Lobby");
            operation!.completed += _ =>
            {
                var lobbyContext = new GameObject("[LOBBY CONTEXT]")
                    .AddComponent<LobbyContext>();
                lobbyContext.Init(_projectContainer);
            };
        }
    }
}