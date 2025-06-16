using System.Collections.Generic;
using System.Linq;
using AuroraWorld.Gameplay.World.Geometry;
using DI;
using R3;
using UnityEngine;
using FogOfWarState = AuroraWorld.Gameplay.World.Data.FogOfWarState;

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
            var terrain = _container.Resolve<WorldStateProxy>().Terrain;

            // Изменение ландшафта на ПКМ и ЛКМ
            _input.ClickUpPosition.Skip(1)
                .Where(data => !data.Modifiers.All && !data.IsPointerOverUI)
                .Subscribe(data =>
                {
                    var hexagonPosition = data.WorldPosition.WorldToHex().ToCube();
                    var worldProxy = container.Resolve<WorldStateProxy>();
                    var changeValue = data.MouseButton switch
                    {
                        0 => 1,
                        1 => -1,
                        _ => 0
                    };
                    var attachedHexagon = worldProxy.Hexagons.GetValueOrDefault(hexagonPosition);
                    if (attachedHexagon != null)
                    {
                        attachedHexagon.WorldInfoProxy.Elevation.Value += changeValue;
                        worldProxy.Terrain.AttachChunkMesh(hexagonPosition);
                    }
                });

            // Выделение гексов на СКМ
            _input.ClickUpPosition.Skip(1)
                .Where(data => !data.Modifiers.All && data is { MouseButton: 2, IsPointerOverUI: false })
                .Subscribe(data =>
                {
                    // selection
                    var hexagonPosition = data.WorldPosition.WorldToHex().ToCube();
                    var worldProxy = container.Resolve<WorldStateProxy>();
                    var positionsInRange = CubeMath.Range(hexagonPosition, selectionSettings.Range.Value);
                    var hexagons = positionsInRange
                        .Select(p => worldProxy.Hexagons.GetValueOrDefault(p))
                        .Where(h => h != null)
                        .ToArray();
                    _mapSelections.AttachSelection("selected", selectionSettings, worldProxy.Terrain, hexagons);
                });

            // Редактирование тумана войны
            _input.ClickUpPosition.Skip(1)
                .Where(data => data.Modifiers.OnlyShift && !data.IsPointerOverUI)
                .Subscribe(data =>
                {
                    var hexagonPosition = data.WorldPosition.WorldToCube();
                    var worldProxy = container.Resolve<WorldStateProxy>();
                    var range = CubeMath.Range(hexagonPosition, selectionSettings.Range.Value);

                    HashSet<Vector3Int> allModifiedChunks = new();
                    foreach (var cube in range)
                    {
                        var fogState = (FogOfWarState)(data.MouseButton + 1);
                        worldProxy.Terrain.AttachHexagon(cube, out var modifiedChunks, fogState);
                        foreach (var chunk in modifiedChunks)
                        {
                            allModifiedChunks.Add(chunk);
                        }
                    }

                    foreach (var modified in allModifiedChunks)
                    {
                        worldProxy.Terrain.AttachChunkMesh(modified);
                    }
                });

            // Движение камеры
            _input.AxesRawUpdate.Skip(1).Subscribe(data =>
            {
                var speed = data.Modifiers.OnlyShift
                    ? _userSettings.CameraSettings.FastMoveSpeed.Value
                    : _userSettings.CameraSettings.MoveSpeed.Value;
                
                var cameraPosition = currentCamera.transform.position;
                float elevation = terrain.GetHexagonInfo(cameraPosition.WorldToCube()).Elevation.Value;
                elevation /= GeometryHexagon.ELEVATION_MODIFER;
                var delta = new Vector3(data.Vector.x, 0, data.Vector.y) * speed;

                var minElevation = elevation + 4;
                var maxElevation = elevation + 32;
                var clampedElevation = Mathf.Clamp(cameraPosition.y, minElevation, maxElevation);
                currentCamera.transform.position = new Vector3(cameraPosition.x, clampedElevation, cameraPosition.z);
                currentCamera.transform.position += delta;
            });
            _input.MouseScroll.Skip(1).Where(data => !data.IsPointerOverUI).Subscribe(data =>
            {
                var cameraPosition = currentCamera.transform.position;
                float elevation = terrain.GetHexagonInfo(cameraPosition.WorldToCube()).Elevation.Value;
                elevation /= GeometryHexagon.ELEVATION_MODIFER;
                if (elevation + 4 > cameraPosition.y - data.Delta || cameraPosition.y - data.Delta > 32) return;
                currentCamera.transform.position += Vector3.down * data.Delta;
            });
        }
    }
}