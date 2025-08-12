using AuroraWorld.GameMenu.Root.View;
using AuroraWorld.Gameplay.Root;
using AuroraWorld.Root;
using DI;
using R3;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AuroraWorld.GameMenu.Root
{
    public class UIGameMenuRoot
    {
        private readonly UIGameMenuRootView _sceneUIView;

        public UIGameMenuRoot(DIContainer container, UIGameMenuRootView sceneUIRootPrefab)
        {
            var sceneUIView = _sceneUIView = Object.Instantiate(sceneUIRootPrefab);
            container.Resolve<UIRoot>().AttachSceneUI(sceneUIView.gameObject);

            sceneUIView.QuitGameSignal.Subscribe(_ => Application.Quit());
        }
        
        public void BindExitSceneSignal(Subject<GameMenuExitParams> exitSceneSignalSubject)
        {
            _sceneUIView.CreateWorldSignal.Subscribe(worldParams =>
            {
                var gameplayEnterParams = new GameplayEnterParams(worldParams.worldName, worldParams.seedHash);
                var exitParams = new GameMenuExitParams(gameplayEnterParams);
                exitSceneSignalSubject.OnNext(exitParams);
            });
            _sceneUIView.OpenWorldSignal.Subscribe(worldName =>
            {
                /*var gameplayEnterParams = new GameplayEnterParams(worldParams.worldName, worldParams.seedHash);
                var exitParams = new GameMenuExitParams(gameplayEnterParams);
                exitSceneSignalSubject.OnNext(exitParams);*/
            });
        }
    }
}