using System;
using AuroraWorld.Gameplay.World.Geometry;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AuroraWorld.Gameplay.World.View
{
    public class WorldEntityView
    {
        public GameObject EntityPrefab { get; }
        public GameObject EntityView { get; }

        public WorldEntityView(GameObject entityPrefab)
        {
            EntityPrefab = entityPrefab;

            EntityView = Object.Instantiate(EntityPrefab);
            EntityView.SetActive(false);
        }

        public void Active(HexagonProxy hexagonProxy)
        {
            var y = hexagonProxy.WorldInfoProxy.Elevation.Value;
            EntityView.transform.position = hexagonProxy.Position.CubeToWorld(y);
            switch (hexagonProxy.WorldInfoProxy.FogOfWar.Value)
            {
                case FogOfWarState.Visible:
                case FogOfWarState.Visited:
                    EntityView.SetActive(true);
                    break;
                case FogOfWarState.Hidden:
                    EntityView.SetActive(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(hexagonProxy.WorldInfoProxy.FogOfWar),
                        hexagonProxy.WorldInfoProxy.FogOfWar.Value, null);
            }
        }
    }
}