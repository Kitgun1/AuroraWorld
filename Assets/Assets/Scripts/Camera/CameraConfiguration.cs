using UnityEngine;

namespace AuroraWorld
{
    public struct CameraConfiguration
    {
        public readonly CinemachineConfiguration CinemachineConfiguration;
        public readonly CameraClearFlags ClearFlags;
        public readonly bool Orthographic;
        public readonly float NearClipPlane;
        public readonly float FarClipPlane;
        public readonly int CullingMask;

        public CameraConfiguration(CinemachineConfiguration cinemachineCamera, (float near, float far) clipPlane,
            CameraClearFlags flags = CameraClearFlags.Skybox, bool orthographic = false, int cullingMask = -1)
        {
            CinemachineConfiguration = cinemachineCamera;
            ClearFlags = flags;
            Orthographic = orthographic;
            NearClipPlane = clipPlane.near;
            FarClipPlane = clipPlane.far;
            CullingMask = cullingMask != -1 ? cullingMask
                : 1 << LayerMask.NameToLayer("Default")
                  & LayerMask.NameToLayer("Hexagon")
                  & LayerMask.NameToLayer("Entity");
        }
    }
}