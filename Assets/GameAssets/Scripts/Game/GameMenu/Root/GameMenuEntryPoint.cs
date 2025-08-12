using AuroraWorld.GameMenu.Root.View;
using DI;
using R3;
using UnityEngine;

namespace AuroraWorld.GameMenu.Root
{
    public class GameMenuEntryPoint : MonoBehaviour
    {
        [SerializeField] private UIGameMenuRootView _sceneUIRootPrefab;
        [SerializeField] private Camera _sceneCamera;

        public Observable<GameMenuExitParams> Run(DIContainer sceneContainer, GameMenuEnterParams enterParams)
        {
            var uiScene = new UIGameMenuRoot(sceneContainer, _sceneUIRootPrefab);
            
            var gameMenuCameraBehaviour = new GameMenuCameraBehaviour(sceneContainer, _sceneCamera);
            
            var exitSceneSignalSubject = new Subject<GameMenuExitParams>();
            uiScene.BindExitSceneSignal(exitSceneSignalSubject);

            return exitSceneSignalSubject;
        }
    }
}