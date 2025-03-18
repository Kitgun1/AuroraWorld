using AuroraWorld.StorageGame;
using DI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AuroraWorld
{
    public static class ProjectContext
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            var projectContainer = new DIContainer();
            projectContainer.RegisterSingleton(_ => new Storage());

            var operation = SceneManager.LoadSceneAsync(0);
            operation!.completed += _ =>
            {
                var lobbyContext = new GameObject("[LOBBY CONTEXT]")
                    .AddComponent<LobbyContext>();
                lobbyContext.Init(projectContainer);
            };
        }
    }
}