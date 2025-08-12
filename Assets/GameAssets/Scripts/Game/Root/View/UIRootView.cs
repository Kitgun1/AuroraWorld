using UnityEngine;

namespace AuroraWorld.Root.View
{
    public class UIRootView : MonoBehaviour
    {
        [SerializeField] private GameObject _loadingScreen;
        [SerializeField] private Transform _uiSceneContainer;
        [SerializeField] private Camera _uiCameraRoot;
        
        public Camera UICameraRoot => _uiCameraRoot;

        private void Awake() => HideLoadingScreen();

        public void ShowLoadingScreen() => _loadingScreen.SetActive(true);
        public void HideLoadingScreen() => _loadingScreen.SetActive(false);
        
        public void AttachSceneUI(GameObject sceneUI)
        {
            ClearSceneUI();
            
            sceneUI.transform.SetParent(_uiSceneContainer, false);
        }

        private void ClearSceneUI()
        {
            var childCount = _uiSceneContainer.childCount;
            for (var i = 0; i < childCount; i++)
            {
                Destroy(_uiSceneContainer.GetChild(i).gameObject);
            }
        }
    }
}