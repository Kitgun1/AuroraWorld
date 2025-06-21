using System;
using ObservableCollections;
using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.World
{
    public class WorldEntityStateProxy
    {
        public readonly ObservableDictionary<string, WorldEntityProxy> Entities;

        public readonly WorldEntityState Origin;

        public WorldEntityStateProxy(WorldEntityState origin)
        {
            Origin = origin;

            Entities = new ObservableDictionary<string, WorldEntityProxy>();
            Origin.Entities.ForEach(e =>
            {
                var id = Guid.NewGuid().ToString();
                var entityProxy = InstanceProxy(e.EntityType);
                Entities.Add(id, entityProxy);
                entityProxy.NextStateSignal.Subscribe(_ => Entities[id] = InstanceProxy(entityProxy.NextState));
            });

            Entities.ObserveAdd().Subscribe(e =>
            {
                Origin.Entities.Add(e.Value.Value.Origin);
                e.Value.Value.NextStateSignal.Subscribe(_ => Entities[e.Value.Key] = InstanceProxy(e.Value.Value.NextState));
            });

            Entities.ObserveRemove().Subscribe(e =>
            {
                Origin.Entities.Remove(e.Value.Value.Origin);
                e.Value.Value.NextStateSignal.Dispose();
            });

            Entities.ObserveReplace().Subscribe(e =>
            {
                e.OldValue.Value.NextStateSignal.Dispose();
                e.NewValue.Value.NextStateSignal.Subscribe(_ => Entities[e.NewValue.Key] = InstanceProxy(e.NewValue.Value.NextState));
            });
        }

        public void InstanceEntity(WorldEntity entity)
        {
            var id = Guid.NewGuid().ToString();
            var proxy = InstanceProxy(entity);
            Entities.Add(id, proxy);
        }

        public void UpdateEntitiesOnMesh(Vector3Int chunkPosition)
        {
            
        }

        private static WorldEntityProxy InstanceProxy(WorldEntity entity)
        {
            return entity switch
            {
                RockEntity rockEntity => new RockEntityProxy(rockEntity),
                _ => throw new Exception($"{entity.EntityType} is not World Entity")
            };
        }
    }
}