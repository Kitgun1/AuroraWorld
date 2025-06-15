using System.Collections.Generic;
using System.Linq;
using AuroraWorld.Gameplay.World.Geometry;
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

        public void Run(DIContainer container)
        {
            _userSettings = new UserSettings();
            _mapSelections = new HexagonMapSelections();
            _input = container.Resolve<UserInput>();
            _container = container;

            var selectionSettings = _userSettings.SelectionSettings;

            _input.ClickPosition.Skip(1).Subscribe(data =>
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
            _input.ClickPosition.Skip(1).Subscribe(data =>
            {
                // selection
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
        }
    }
}