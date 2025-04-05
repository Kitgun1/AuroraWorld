using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Assets.Scripts.Game.Gameplay.World
{
    public class WorldSample : MonoBehaviour
    {
        [SerializeField] private GameObject _hexagonObj;

        public Vector2Int SizeMapDemo = new Vector2Int(100, 100);
        public GenerationSettings GenerationSettings = new();

        private WorldStateProxy _worldStateProxy;

        private readonly List<GameObject> _cachedHexagonObjs = new();

        private void Start()
        {
            WorldState worldState = new WorldState();
            _worldStateProxy = new WorldStateProxy(worldState);

            _worldStateProxy.Hexagons.ObserveAdd().Subscribe(e =>
            {
                var hexagonPosition = e.Value.Position;
                var x = hexagonPosition.x + (hexagonPosition.y % 2 == 0 ? 0 : 0.50f);
                var y = hexagonPosition.y * 0.75f;
                var worldPosition = new Vector3(x, 0, y);
                var hexagonEntityObj = Instantiate(_hexagonObj, worldPosition, Quaternion.identity, transform);
                hexagonEntityObj.name = $"Hexagon [{hexagonPosition.x}, {hexagonPosition.y}]";

                var material = new Material(hexagonEntityObj.GetComponent<MeshRenderer>().material)
                {
                    color = e.Value.BiomeType.ToColor()
                };


                hexagonEntityObj.GetComponent<MeshRenderer>().material = material;
                _cachedHexagonObjs.Add(hexagonEntityObj);
            });

            _worldStateProxy.Hexagons.ObserveClear().Subscribe(e =>
            {
                foreach (var hexagonObj in _cachedHexagonObjs)
                {
                    Destroy(hexagonObj);
                }
            });
        }

        private IEnumerator _generatorEnumerator;

        private void Update()
        {
            if (Input.GetKey(KeyCode.R))
            {
                if (_generatorEnumerator != null)
                {
                    StopCoroutine(_generatorEnumerator);
                    _generatorEnumerator = null;
                }

                _generatorEnumerator = GenerateMap();
                StartCoroutine(_generatorEnumerator);
            }
        }

        private IEnumerator GenerateMap()
        {
            _worldStateProxy.Hexagons.Clear();

            WorldStateExtensions.UpdateRandomSeed();
            for (int x = 0; x < SizeMapDemo.x; x++)
            {
                for (int y = 0; y < SizeMapDemo.y; y++)
                {
                    var pos = new Vector2Int(x, y);
                    var biome = GenerationSettings.GetBiomeBySettings(pos);
                    _worldStateProxy.Hexagons.Add(new HexagonEntityProxy(new HexagonEntity(pos, biome)));
                }

                yield return null;
            }
        }
    }
}