using Assets.Utils.Coroutine;
using AuroraWorld.Gameplay.World.Geometry;
using DI;
using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.Player
{
    public class User : MonoBehaviour
    {
        private UserInput _input;
        private DIContainer _container;
            
        public void Run(DIContainer container)
        {
            _input = container.Resolve<UserInput>();
            _container = container;

            var outlineObject = new GameObject("Outline");
            //_outlineMeshFilter = outlineObject.AddComponent<MeshFilter>();
            outlineObject.AddComponent<MeshRenderer>().material = Resources.Load<Material>("Vertex Material");
            //_outlineThickLineMesh = new ThickLineMesh(0.1f);
            
            _input.ClickPosition.Skip(1).Subscribe(data =>
            {
                var hexagonPosition = data.WorldPosition.WorldToHex().ToCube();
                var worldProxy = container.Resolve<WorldStateProxy>();
                var changeValue = data.MouseButton switch
                {
                    0 => 0.02f,
                    1 => -0.02f,
                    _ => 0
                };
                worldProxy.Hexagons[hexagonPosition].WorldInfoProxy.Elevation.Value += changeValue;
                worldProxy.AttachHexagon(hexagonPosition);
            });
            _input.ClickPosition.Skip(1).Subscribe(data =>
            {
                if(data.MouseButton != 2) return;
                var hexagonPosition = data.WorldPosition.WorldToHex().ToCube();
                var worldProxy = container.Resolve<WorldStateProxy>();
                /*_selectedHexagons.Add(worldProxy.Hexagons[hexagonPosition]);
                _outlineThickLineMesh.Construct(_selectedHexagons.ToArray());
                _outlineMeshFilter.mesh = _outlineThickLineMesh.Mesh;*/
            });
        }
    }
}