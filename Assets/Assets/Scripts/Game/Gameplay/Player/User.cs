using System.Linq;
using AuroraWorld.Gameplay.World.Geometry;
using AuroraWorld.Gameplay.World.Proxy;
using AuroraWorld.Gameplay.World.Terrain;
using DI;
using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.Player
{
    public class User : MonoBehaviour
    {
        private UserInput _input;
        private HexagonMapSelections _mapSelections;
        private DIContainer _container;

        private UserSettings _userSettings;

        public void Run(DIContainer container, Camera currentCamera)
        {
            _userSettings = new UserSettings();
            _mapSelections = new HexagonMapSelections();
            _input = container.Resolve<UserInput>();
            _container = container;

            var selectionSettings = _userSettings.SelectionSettings;
            var worldStateProxy = _container.Resolve<WorldStateProxy>();
            var terrain = worldStateProxy.TerrainState;

            // Изменение ландшафта на ПКМ и ЛКМ
            _input.MouseClickUp.Skip(1)
                .Where(data => !data.Modifiers.Any && !data.IsPointerOverUI)
                .Subscribe(data =>
                {
                    var hexagonPosition = data.WorldPosition.WorldToHex().ToCube();
                    if (!terrain.ContainsLoadedHexagon(hexagonPosition)) return;
                    var changeValue = data.MouseButton switch { 0 => 1, 1 => -1, _ => 0 };
                    var attachedHexagon = terrain.LoadHexagon(hexagonPosition);
                    attachedHexagon.GeographyInfo.Elevation.Value += changeValue;

                    terrain.UpdateDirtyHexagons();
                });

            // Выделение гексов на СКМ
            _input.MouseClickUp.Skip(1)
                .Where(data => !data.Modifiers.Any && data is { MouseButton: 2, IsPointerOverUI: false })
                .Subscribe(data =>
                {
                    // selection
                    var hexagonPosition = data.WorldPosition.WorldToHex().ToCube();
                    var positionsInRange = CubeMath.Range(hexagonPosition, selectionSettings.Range.Value);
                    var hexagonProxies = positionsInRange.Select(terrain.LoadHexagon).ToArray();
                    _mapSelections.AttachSelection("selected", selectionSettings, terrain, hexagonProxies);
                });

            // Редактирование тумана войны
            _input.MouseClickUp.Skip(1)
                .Where(data => data.Modifiers.OnlyShift && !data.IsPointerOverUI)
                .Subscribe(data =>
                {
                    var hexagonPosition = data.WorldPosition.WorldToCube();
                    var range = CubeMath.Range(hexagonPosition, selectionSettings.Range.Value);
                    var targetFogOfState = (FogOfWarState)data.MouseButton;
                    foreach (var cube in range)
                    {
                        var hexagon = terrain.LoadHexagon(cube);
                        hexagon.FogOfWarState.Value = targetFogOfState;
                        if (!terrain.ContainsLoadedHexagon(cube))
                        {
                            terrain.AddHexagon(hexagon);
                        }
                    }
                    terrain.UpdateDirtyHexagons();
                });

            // Движение камеры
            _input.AxesRawUpdate.Skip(1).Subscribe(data =>
            {
                var speed = data.Modifiers.OnlyShift
                    ? _userSettings.CameraSettings.FastMoveSpeed.Value
                    : _userSettings.CameraSettings.MoveSpeed.Value;

                var cameraPosition = currentCamera.transform.position;
                float elevation = terrain.LoadHexagon(cameraPosition.WorldToCube()).GeographyInfo.Elevation.Value;
                elevation /= GeometryHexagon.ELEVATION_MODIFER;
                var delta = new Vector3(data.Vector.x, 0, data.Vector.y) * speed;

                var minElevation = elevation + 4;
                var maxElevation = elevation + 24;
                var clampedElevation = Mathf.Clamp(cameraPosition.y, minElevation, maxElevation);
                currentCamera.transform.position = new Vector3(cameraPosition.x, clampedElevation, cameraPosition.z);
                currentCamera.transform.position += delta;
            });
            _input.MouseScroll.Skip(1).Where(data => !data.IsPointerOverUI).Subscribe(data =>
            {
                var cameraPosition = currentCamera.transform.position;
                float elevation = terrain.LoadHexagon(cameraPosition.WorldToCube()).GeographyInfo.Elevation.Value;
                elevation /= GeometryHexagon.ELEVATION_MODIFER;
                if (elevation + 4 > cameraPosition.y - data.Delta || cameraPosition.y - data.Delta > 24) return;
                currentCamera.transform.position += Vector3.down * data.Delta;
            });
        }
    }
}