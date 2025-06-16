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

            // Изменение ландшафта на ПКМ и ЛКМ
            _input.ClickUpPosition.Skip(1).Subscribe(data =>
            {
                if (data.Modifiers.Any) return;
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
            _input.ClickUpPosition.Skip(1).Subscribe(data =>
            {
                // selection
                if (data.Modifiers.Any) return;
                if (data.MouseButton != 2) return;
                var hexagonPosition = data.WorldPosition.WorldToHex().ToCube();
                var worldProxy = container.Resolve<WorldStateProxy>();
                var positionsInRange = CubeMath.Range(hexagonPosition, selectionSettings.Range.Value);
                var hexagons = positionsInRange
                    .Select(p => worldProxy.Hexagons.GetValueOrDefault(p))
                    .Where(h => h != null)
                    .ToArray();
                _mapSelections.AttachSelection("selected", selectionSettings, worldProxy.Terrain, hexagons);
            });
            _input.MouseMovedToHexagon.Skip(1).Subscribe(data => { });

            // Редактирование тумана войны
            _input.ClickUpPosition.Skip(1).Subscribe(data =>
            {
                if (!data.Modifiers.OnlyShift) return;

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

            _input.AxesRawUpdate.Skip(1).Subscribe(data =>
            {
                var speed = data.Modifiers.OnlyShift
                    ? _userSettings.CameraSettings.FastMoveSpeed.Value
                    : _userSettings.CameraSettings.MoveSpeed.Value;
                var delta = new Vector3(data.Vector.x, 0, data.Vector.y) * speed;
                currentCamera.transform.position += delta;
            });
        }
    }
}