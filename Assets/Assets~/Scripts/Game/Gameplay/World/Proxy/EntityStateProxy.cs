using System;
using System.Collections.Generic;
using System.Linq;
using AuroraWorld.App.GameResources;
using AuroraWorld.Gameplay.World.Entity;
using AuroraWorld.Gameplay.World.Entity.Proxy;
using AuroraWorld.Gameplay.World.Geometry;
using AuroraWorld.Gameplay.World.Terrain.Proxy;
using DI;
using ObservableCollections;
using R3;
using UnityEngine;
using Object = System.Object;

namespace AuroraWorld.Gameplay.World.Proxy
{
    public class EntityStateProxy
    {
        public readonly ObservableDictionary<string, WorldEntityProxy> Entities;

        private readonly List<GameObject> _entityObjects = new List<GameObject>();

        private readonly DIContainer _container;
        private readonly Resource<GameObject> entityPrefabResources;
        private readonly EntityState _origin;

        public EntityStateProxy(DIContainer container, EntityState origin)
        {
            _container = container;
            _origin = origin;
            entityPrefabResources = new Resource<GameObject>();

            Entities = new ObservableDictionary<string, WorldEntityProxy>();
            _origin.Entities.ForEach(entity =>
            {
                var id = Guid.NewGuid().ToString();
                Entities.Add(id, WorldEntityProxy.InstanceProxy(entity));

                // TODO: Инициализация остальных параметров
            });

            Entities.ObserveAdd().Subscribe(entity => _origin.Entities.Add(entity.Value.Value.Origin));
            Entities.ObserveRemove().Subscribe(entity => _origin.Entities.Remove(entity.Value.Value.Origin));

            // TODO: обновить состояние данных при обновлении состояния энтити
        }

        public WorldEntityProxy LoadEntity(Vector3Int cubePosition, GeographyInfoProxy hexagonGeographyInfo)
        {
            if (hexagonGeographyInfo.Vegetation.Value > 0.5f)
            {
                var entity = WorldEntityFactory.InstanceSmallRockEntity(cubePosition);
                var entityProxy = new RockEntityProxy(entity);
                Entities.Add(new KeyValuePair<string, WorldEntityProxy>(Guid.NewGuid().ToString(), entityProxy));
                return entityProxy;
            }

            return null;
        }

        public void UpdateEntityVisible()
        {
            var terrainState = _container.Resolve<WorldStateProxy>().TerrainState;
            foreach (var pair in Entities)
            {
                var entityProxy = pair.Value;
                if (!entityProxy.ControlledCubePositions.All(controlledPosition => terrainState.ContainsLoadedHexagon(controlledPosition))) continue;

                var prefab = entityPrefabResources.Load("Prefabs/Entity/Rock");
                var elevation = _container.Resolve<WorldStateProxy>().TerrainState.LoadHexagon(entityProxy.ControlledCubePositions[0]).GeographyInfo.Elevation.Value;
                _entityObjects.Add(UnityEngine.Object.Instantiate(prefab, entityProxy.ControlledCubePositions[0].CubeToWorld(elevation), Quaternion.identity));
            }
        }
    }
}