using System;
using AuroraWorld.Gameplay.World.Entity.Proxy;
using ObservableCollections;
using R3;

namespace AuroraWorld.Gameplay.World.Proxy
{
    public class EntityStateProxy
    {
        public readonly ObservableDictionary<string, WorldEntityProxy> Entities;

        private readonly EntityState _origin;

        public EntityStateProxy(EntityState origin)
        {
            _origin = origin;

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
    }
}