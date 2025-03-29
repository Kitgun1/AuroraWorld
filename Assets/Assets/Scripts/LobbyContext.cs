using DI;
using UnityEngine;

namespace AuroraWorld
{
    public class LobbyContext : MonoBehaviour
    {
        private DIContainer _lobbyContainer;

        public void Init(DIContainer projectContainer)
        {
            _lobbyContainer = new DIContainer(projectContainer);

            _lobbyContainer.RegisterSingleton(UI_CAMERA_CONFIG, UICameraConfigFactory);
            _lobbyContainer.RegisterSingleton(UI_CAMERA, UICameraFactory);
        }

        #region Factories

        private const string UI_CAMERA_CONFIG = nameof(UI_CAMERA_CONFIG);
        private const string UI_CAMERA = nameof(UI_CAMERA);
        
        private static Camera UICameraFactory(DIContainer c) => new(c.Resolve<CameraConfiguration>(UI_CAMERA_CONFIG));

        private static CameraConfiguration UICameraConfigFactory(DIContainer _)
            => new(null, (0, 10), CameraClearFlags.Nothing, true, 1 << LayerMask.NameToLayer("UI"));

        #endregion
    }
}