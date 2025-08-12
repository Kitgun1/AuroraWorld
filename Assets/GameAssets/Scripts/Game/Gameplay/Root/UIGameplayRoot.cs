using AuroraWorld.Gameplay.Root.View;
using AuroraWorld.Root;
using DI;
using UnityEngine;

namespace AuroraWorld.Gameplay.Root
{
    public class UIGameplayRoot
    {
        private readonly UIGameplayRootView _sceneUIView;
        
        public UIGameplayRoot(DIContainer container, UIGameplayRootView sceneUIRootPrefab)
        {
            var sceneUIView = _sceneUIView = Object.Instantiate(sceneUIRootPrefab);
            container.Resolve<UIRoot>().AttachSceneUI(sceneUIView.gameObject);

            
        }

    }
}