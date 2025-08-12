using AuroraWorld.Root.View;
using DI;
using UnityEngine;

namespace AuroraWorld.Root
{
    public class UIRoot
    {
        private readonly DIContainer _container;
        private readonly UIRootView _uiRootView;
        
        public Camera UICameraRoot => _uiRootView.UICameraRoot;
        
        public UIRoot(DIContainer container)
        {
            _container = container;
            var uiRootViewPrefab = Resources.Load<UIRootView>("Prefabs/UIRoot");
            _uiRootView = Object.Instantiate(uiRootViewPrefab);
            Object.DontDestroyOnLoad(_uiRootView.gameObject);

            _container.RegisterInstance(this);
        }

        public void ShowLoadingScreen() => _uiRootView.ShowLoadingScreen();
        public void HideLoadingScreen() => _uiRootView.HideLoadingScreen();

        public void AttachSceneUI(GameObject sceneUI) => _uiRootView.AttachSceneUI(sceneUI);
    }
}