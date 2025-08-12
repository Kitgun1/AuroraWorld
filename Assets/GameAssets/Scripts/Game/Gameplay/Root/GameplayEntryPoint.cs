using AuroraWorld.Gameplay.Root.View;
using DI;
using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.Root
{
    public class GameplayEntryPoint : MonoBehaviour
    {
        [SerializeField] private UIGameplayRootView _sceneUIRootPrefab;

        public Observable<GameplayExitParams> Run(DIContainer sceneContainer, GameplayEnterParams enterParams)
        {
            var uiScene = new UIGameplayRoot(sceneContainer, _sceneUIRootPrefab);

            var exitSceneSignalSubject = new Subject<GameplayExitParams>();
            // TODO: Забиндить на сцене выход из сцены

            return exitSceneSignalSubject;
        }
    }
}