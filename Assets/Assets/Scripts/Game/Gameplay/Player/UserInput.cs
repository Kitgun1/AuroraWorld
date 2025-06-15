using AuroraWorld.Gameplay.Player.InputData;
using AuroraWorld.Gameplay.World.Geometry;
using DI;
using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.Player
{
    public class UserInput
    {
        public readonly ReactiveProperty<ClickData> ClickPosition = new();
        public readonly Subject<MouseMoveData> MouseMove = new();
        public readonly Subject<MouseMovedToHexagon> MouseMovedToHexagon = new();

        private DIContainer _container;
        private Camera _camera;

        public void Run(DIContainer container, MonoBehaviour monoBehaviour, Camera currentCamera)
        {
            _container = container;
            _camera = currentCamera;

            for (int i = 0; i < 3; i++)
            {
                var index = i;
                Observable.EveryUpdate()
                    .Where(_ => Input.GetMouseButtonUp(index))
                    .Subscribe(_ => ClickHandler(index))
                    .AddTo(monoBehaviour);
            }

            Vector3 oldMousePosition = Vector3.zero;
            Observable.EveryUpdate()
                .Where(_ => Input.mousePosition != oldMousePosition)
                .Subscribe(_ =>
                {
                    oldMousePosition = Input.mousePosition;
                    MouseMove.OnNext(new MouseMoveData
                    {
                        ScreenPosition = oldMousePosition,
                        WorldPosition = ToWorldPosition(oldMousePosition)
                    });
                })
                .AddTo(monoBehaviour);

            var oldHexagonPosition = Vector3Int.zero;
            Observable.EveryUpdate()
                .Where(_ => Input.mousePosition.WorldToHex().ToCube() != oldHexagonPosition)
                .Subscribe(_ =>
                {
                    oldHexagonPosition = Input.mousePosition.WorldToHex().ToCube();
                    MouseMovedToHexagon.OnNext(new MouseMovedToHexagon
                    {
                        ScreenPosition = Input.mousePosition,
                        CubePosition = oldHexagonPosition
                    });
                })
                .AddTo(monoBehaviour);
        }

        private void ClickHandler(int index)
        {
            var screenPosition = Input.mousePosition;
            var worldPoint = ToWorldPosition(screenPosition);
            ClickPosition.Value = new ClickData(index, screenPosition, worldPoint,
                ctrl: Input.GetKey(KeyCode.LeftControl),
                shift: Input.GetKey(KeyCode.LeftShift),
                alt: Input.GetKey(KeyCode.LeftAlt));
        }

        private Vector3 ToWorldPosition(Vector3 screenPosition)
        {
            var terrain = _container.Resolve<WorldStateProxy>().Terrain;
            var ray = _camera.ScreenPointToRay(screenPosition);
            RaycastHit[] hits = new RaycastHit[1];
            var isHit = Physics.RaycastNonAlloc(ray, hits, 300, ~0) > 0;

            Vector3 worldPoint;
            if (isHit)
            {
                worldPoint = hits[0].point;
            }
            else
            {
                var distanceToGround = ray.origin.y / -ray.direction.y;
                worldPoint = ray.origin + ray.direction * distanceToGround;
                worldPoint.y = 0;
                var line = CubeMath.Line(ray.origin.WorldToCube(), worldPoint.WorldToCube());
                foreach (var cubePosition in line)
                {
                    var elevation = terrain.GetHexagonInfo(cubePosition).Elevation.Value / GeometryHexagon.ELEVATION_MODIFER;
                    distanceToGround = (ray.origin.y - elevation) / -ray.direction.y;
                    worldPoint = ray.origin + ray.direction * distanceToGround;
                    worldPoint.y = elevation;
                    if (worldPoint.WorldToCube() == cubePosition) return worldPoint;
                }
            }

            return worldPoint;
        }
    }
}