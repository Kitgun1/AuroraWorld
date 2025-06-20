using System;
using System.Linq;
using AuroraWorld.Gameplay.Player.InputData;
using AuroraWorld.Gameplay.World;
using AuroraWorld.Gameplay.World.Geometry;
using DI;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AuroraWorld.Gameplay.Player
{
    public class UserInput
    {
        public readonly Subject<ClickData> ClickPosition = new();
        public readonly Subject<ClickData> ClickUpPosition = new();
        public readonly Subject<ClickData> ClickDownPosition = new();
        public readonly Subject<MouseScrollData> MouseScroll = new();
        public readonly Subject<MouseMoveData> MouseMove = new();
        public readonly Subject<MouseMovedToHexagon> MouseMovedToHexagon = new();
        public readonly Subject<KeyboardData> KeyboardClick = new();
        public readonly Subject<KeyboardData> KeyboardClickUp = new();
        public readonly Subject<AxesData> AxesUpdate = new();
        public readonly Subject<AxesData> AxesRawUpdate = new();

        private DIContainer _container;
        private Camera _camera;

        private static readonly KeyCode[] IgnoredKeys =
        {
            KeyCode.LeftAlt, KeyCode.RightAlt, KeyCode.LeftControl, KeyCode.RightControl,
            KeyCode.LeftShift, KeyCode.RightShift, KeyCode.CapsLock, KeyCode.LeftWindows,
            KeyCode.RightWindows, KeyCode.Numlock, KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Mouse2,
            KeyCode.Mouse3, KeyCode.Mouse4, KeyCode.Mouse5, KeyCode.Mouse6, KeyCode.ScrollLock
        };

        public void Run(DIContainer container, MonoBehaviour monoBehaviour, Camera currentCamera)
        {
            _container = container;
            _camera = currentCamera;

            for (int i = 0; i < 6; i++)
            {
                var index = i;
                Observable.EveryUpdate()
                    .Where(_ => Input.GetMouseButtonUp(index))
                    .Subscribe(_ => ClickHandler(ClickUpPosition, index))
                    .AddTo(monoBehaviour);
                Observable.EveryUpdate()
                    .Where(_ => Input.GetMouseButton(index))
                    .Subscribe(_ => ClickHandler(ClickPosition, index))
                    .AddTo(monoBehaviour);
                Observable.EveryUpdate()
                    .Where(_ => Input.GetMouseButtonDown(index))
                    .Subscribe(_ => ClickHandler(ClickDownPosition, index))
                    .AddTo(monoBehaviour);
            }

            float oldScrollDelta = 0;
            Observable.EveryUpdate().Where(_ => oldScrollDelta != Input.mouseScrollDelta.y)
                .Subscribe(_ =>
                {
                    MouseScroll.OnNext(new MouseScrollData()
                    {
                        Delta = oldScrollDelta = Input.mouseScrollDelta.y,
                        Modifiers = GetCurrentModifiers(),
                        IsPointerOverUI = EventSystem.current.IsPointerOverGameObject()
                    });
                }).AddTo(monoBehaviour);

            Vector3 oldMousePosition = Vector3.zero;
            Observable.EveryUpdate().Where(_ => Input.mousePosition != oldMousePosition)
                .Subscribe(_ =>
                {
                    oldMousePosition = Input.mousePosition;
                    MouseMove.OnNext(new MouseMoveData
                    {
                        ScreenPosition = oldMousePosition,
                        WorldPosition = ToWorldPosition(oldMousePosition),
                        IsPointerOverUI = EventSystem.current.IsPointerOverGameObject()
                    });
                })
                .AddTo(monoBehaviour);

            var oldHexagonPosition = Vector3Int.zero;
            Observable.EveryUpdate().Where(_ => Input.mousePosition.WorldToHex().ToCube() != oldHexagonPosition)
                .Subscribe(_ =>
                {
                    oldHexagonPosition = Input.mousePosition.WorldToHex().ToCube();
                    MouseMovedToHexagon.OnNext(new MouseMovedToHexagon
                    {
                        ScreenPosition = Input.mousePosition,
                        CubePosition = oldHexagonPosition,
                        IsPointerOverUI = EventSystem.current.IsPointerOverGameObject()
                    });
                })
                .AddTo(monoBehaviour);

            #region Keyboard actions

            Observable.EveryUpdate().Where(_ => Input.anyKey)
                .Subscribe(_ =>
                {
                    var keyboardData = new KeyboardData
                    {
                        Modifiers = GetCurrentModifiers()
                    };
                    foreach (KeyCode keyKode in Enum.GetValues(typeof(KeyCode)))
                    {
                        if (IgnoredKeys.Contains(keyKode)) continue;
                        if (Input.GetKey(keyKode))
                        {
                            keyboardData.KeyCode = keyKode;
                            KeyboardClick.OnNext(keyboardData);
                        }
                    }
                });

            Observable.EveryUpdate().Where(_ => Input.anyKeyDown)
                .Subscribe(_ =>
                {
                    var keyboardData = new KeyboardData
                    {
                        Modifiers = GetCurrentModifiers()
                    };
                    foreach (KeyCode keyKode in Enum.GetValues(typeof(KeyCode)))
                    {
                        if (IgnoredKeys.Contains(keyKode)) continue;
                        if (Input.GetKeyUp(keyKode))
                        {
                            keyboardData.KeyCode = keyKode;
                            KeyboardClickUp.OnNext(keyboardData);
                        }
                    }
                });

            #endregion

            #region Axis actions

            Observable.EveryUpdate().Subscribe(_ =>
                {
                    AxesUpdate.OnNext(new AxesData
                    {
                        Horizontal = Input.GetAxis("Horizontal"),
                        Vertical = Input.GetAxis("Vertical"),
                        Modifiers = GetCurrentModifiers()
                    });
                })
                .AddTo(monoBehaviour);

            Observable.EveryUpdate().Subscribe(_ =>
                {
                    AxesRawUpdate.OnNext(new AxesData
                    {
                        Horizontal = Input.GetAxisRaw("Horizontal"),
                        Vertical = Input.GetAxisRaw("Vertical"),
                        Modifiers = GetCurrentModifiers()
                    });
                })
                .AddTo(monoBehaviour);

            #endregion
        }

        private void ClickHandler(Subject<ClickData> property, int index)
        {
            var screenPosition = Input.mousePosition;
            var worldPoint = ToWorldPosition(screenPosition);
            property.OnNext(new ClickData(index, screenPosition, worldPoint, GetCurrentModifiers(),
                EventSystem.current.IsPointerOverGameObject()));
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
                    var elevation = terrain.GetHexagonInfo(cubePosition).Elevation / GeometryHexagon.ELEVATION_MODIFER;
                    distanceToGround = (ray.origin.y - elevation) / -ray.direction.y;
                    worldPoint = ray.origin + ray.direction * distanceToGround;
                    worldPoint.y = elevation;
                    if (worldPoint.WorldToCube() == cubePosition) return worldPoint;
                }
            }

            return worldPoint;
        }

        private Modifiers GetCurrentModifiers() => new()
        {
            HasAlt = Input.GetKey(KeyCode.LeftAlt),
            HasCtrl = Input.GetKey(KeyCode.LeftControl),
            HasShift = Input.GetKey(KeyCode.LeftShift),
            HasCapsLock = Input.GetKey(KeyCode.CapsLock)
        };
    }
}