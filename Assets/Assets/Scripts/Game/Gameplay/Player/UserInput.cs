using System;
using System.Collections.Generic;
using System.Linq;
using AuroraWorld.Gameplay.Player.InputData;
using AuroraWorld.Gameplay.World.Geometry;
using AuroraWorld.Gameplay.World.Proxy;
using DI;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AuroraWorld.Gameplay.Player
{
    public class UserInput
    {
        public readonly Subject<ClickData> MouseClick = new();
        public readonly Subject<ClickData> MouseClickUp = new();
        public readonly Subject<ClickData> MouseClickDown = new();
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

        private static IEnumerable<KeyCode> _allowKeys;

        public void Run(DIContainer container, MonoBehaviour monoBehaviour, Camera currentCamera)
        {
            _container = container;
            _camera = currentCamera;

            _allowKeys = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().Where(k => !IgnoredKeys.Contains(k));

            for (int i = 0; i < 6; i++)
            {
                var index = i;
                Observable.EveryUpdate().Subscribe(_ =>
                    {
                        if (Input.GetMouseButtonUp(index)) ClickHandler(MouseClickUp, index);
                        if (Input.GetMouseButton(index)) ClickHandler(MouseClick, index);
                        if (Input.GetMouseButtonDown(index)) ClickHandler(MouseClickDown, index);
                    })
                    .AddTo(monoBehaviour);
            }

            float oldScrollDelta = 0;
            Vector3 oldMousePosition = Vector3.zero;
            var oldHexagonPosition = Vector3Int.zero;
            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    if(oldScrollDelta != Input.mouseScrollDelta.y)
                    {
                        MouseScroll.OnNext(new MouseScrollData()
                        {
                            Delta = oldScrollDelta = Input.mouseScrollDelta.y,
                            Modifiers = GetCurrentModifiers(),
                            IsPointerOverUI = EventSystem.current.IsPointerOverGameObject()
                        });
                    }
                    if(Input.mousePosition != oldMousePosition)
                    {
                        oldMousePosition = Input.mousePosition;
                        MouseMove.OnNext(new MouseMoveData
                        {
                            ScreenPosition = oldMousePosition,
                            WorldPosition = ToWorldPosition(oldMousePosition),
                            IsPointerOverUI = EventSystem.current.IsPointerOverGameObject()
                        });
                    }

                    if (Input.mousePosition.WorldToHex().ToCube() != oldHexagonPosition)
                    {
                        oldHexagonPosition = Input.mousePosition.WorldToHex().ToCube();
                        MouseMovedToHexagon.OnNext(new MouseMovedToHexagon
                        {
                            ScreenPosition = Input.mousePosition,
                            CubePosition = oldHexagonPosition,
                            IsPointerOverUI = EventSystem.current.IsPointerOverGameObject()
                        });
                    }
                }).AddTo(monoBehaviour);

            #region Keyboard actions

            Observable.EveryUpdate()
                .Where(_ => Input.anyKey || Input.anyKeyDown)
                .Subscribe(_ =>
                {
                    var keyboardData = new KeyboardData
                    {
                        Modifiers = GetCurrentModifiers()
                    };
                   
                    foreach (KeyCode keyKode in _allowKeys)
                    {
                        if (Input.GetKey(keyKode))
                        {
                            keyboardData.KeyCode = keyKode;
                            KeyboardClick.OnNext(keyboardData);
                        }

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
            property.OnNext(new ClickData(index, screenPosition, worldPoint, GetCurrentModifiers(), EventSystem.current.IsPointerOverGameObject()));
        }

        private Vector3 ToWorldPosition(Vector3 screenPosition)
        {
            var terrain = _container.Resolve<WorldStateProxy>().TerrainState;
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
                    var geographyInfo = terrain.LoadHexagon(cubePosition).GeographyInfo;
                    var elevation = geographyInfo.Elevation.Value / GeometryHexagon.ELEVATION_MODIFER;
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