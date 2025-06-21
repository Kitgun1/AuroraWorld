using AuroraWorld.Gameplay.World.View;
using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.World
{
    public abstract class WorldEntityProxy
    {
        public readonly string Name;
        public readonly Vector3Int[] Positions;
        public readonly ReactiveProperty<int> MaxHealth;
        public readonly ReactiveProperty<int> Health;
        // TODO: Ресурсы, которые будут получены после добычи пешкой
        public readonly WorldEntity NextState;
        
        public readonly WorldEntity Origin;
        
        public readonly Subject<Unit> NextStateSignal;
        
        public WorldEntityView View { get; protected set; }

        public WorldEntityProxy(WorldEntity origin)
        {
            Origin = origin;

            Name = Origin.Name;
            Positions = Origin.Positions;
            MaxHealth = new ReactiveProperty<int>(Origin.MaxHealth);
            Health = new ReactiveProperty<int>(Origin.Health);
            NextState = Origin.NextState;
            NextStateSignal = new Subject<Unit>();

            MaxHealth.Skip(1).Subscribe(e =>
            {
                e = Mathf.Clamp(e, -1, int.MaxValue);
                if (e == -1)
                {
                    Origin.MaxHealth = e;
                    Origin.Health = e;
                    return;
                }
                var percentHealth = (float)Health.Value / Origin.MaxHealth;
                Origin.MaxHealth = e;
                Health.Value = (int)(percentHealth * e);
            });

            Health.Skip(1).Subscribe(e =>
            {
                if (MaxHealth.Value == -1)
                {
                    Origin.Health = -1;
                    return;
                }
                Origin.Health = Mathf.Clamp(e, 0, MaxHealth.Value);
                if(Origin.Health == 0) OnDestruction();
            });
        }

        public virtual void OnDestruction()
        {
            if(NextState == null) return;
            // TODO: Ставим вместо себя следующее состояние объекта 
            NextStateSignal.OnNext(Unit.Default);
            NextStateSignal.OnCompleted();
        }
    }
}