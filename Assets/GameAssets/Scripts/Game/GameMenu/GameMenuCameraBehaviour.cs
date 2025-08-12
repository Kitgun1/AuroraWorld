using AuroraWorld.Root;
using DI;
using R3;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace AuroraWorld.GameMenu
{
    public class GameMenuCameraBehaviour
    {
        public GameMenuCameraBehaviour(DIContainer sceneContainer, Camera camera)
        {
            var cameraData = camera.GetUniversalAdditionalCameraData();
            cameraData.cameraStack.Add(sceneContainer.Resolve<UIRoot>().UICameraRoot);

            var pivotCamera = cameraData.transform.parent;
            const float rotatePerSecond = 1 / 60f;
            Observable.EveryUpdate().Subscribe(_ =>
            {
                pivotCamera?.Rotate(new Vector3(0, 360f, 0) * (Time.deltaTime * rotatePerSecond), Space.Self);
            });
        }
    }
}