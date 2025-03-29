using Cinemachine;
using UnityEngine;

namespace AuroraWorld
{
    public class Camera
    {
        public readonly CinemachineVirtualCamera VirtualCamera;
        public readonly UnityEngine.Camera UnityCamera;
        private readonly CameraConfiguration _config;

        public Camera(CameraConfiguration config)
        {
            _config = config;

            var parentCamera = new GameObject("[CAMERA]");
            UnityCamera = InstanceCamera(parentCamera);
            if (_config.CinemachineConfiguration == null)
            {
                VirtualCamera = null;
                return;
            }

            VirtualCamera = InstanceCinemachineCamera(parentCamera);
        }

        private CinemachineVirtualCamera InstanceCinemachineCamera(GameObject parent)
        {
            UnityCamera.gameObject.AddComponent<CinemachineBrain>();
            var virtualCamera = new GameObject("Virtual Camera").AddComponent<CinemachineVirtualCamera>();
            virtualCamera.transform.parent = parent.transform;
            virtualCamera.Follow = _config.CinemachineConfiguration.Follow;
            virtualCamera.LookAt = _config.CinemachineConfiguration.LookAt;
            var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
            transposer.m_FollowOffset = _config.CinemachineConfiguration.FollowOffset;
            return virtualCamera;
        }

        private UnityEngine.Camera InstanceCamera(GameObject parent)
        {
            var camera = new GameObject("Camera").AddComponent<UnityEngine.Camera>();
            camera.transform.parent = parent.transform;
            camera.cullingMask = _config.CullingMask;
            camera.orthographic = _config.Orthographic;
            camera.farClipPlane = _config.FarClipPlane;
            camera.nearClipPlane = _config.NearClipPlane;
            camera.clearFlags = _config.ClearFlags;
            return camera;
        }
    }
}