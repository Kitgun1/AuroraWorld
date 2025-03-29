using UnityEngine;

namespace AuroraWorld
{
    public class CinemachineConfiguration
    {
        public readonly Transform Follow;
        public readonly Transform LookAt;
        public readonly Vector3 FollowOffset;

        public CinemachineConfiguration(Transform follow, Transform lookAt, Vector3 followOffset)
        {
            Follow = follow;
            LookAt = lookAt;
            FollowOffset = followOffset;
        }
    }
}