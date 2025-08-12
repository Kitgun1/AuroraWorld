using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.GameColony
{
    public class BodyComponentProxy
    {
        public string Name => Origin.Name;
        public ReactiveProperty<int> MaxHealth;
        public ReactiveProperty<int> Health;

        public BodyComponent Origin;

        public BodyComponentProxy(BodyComponent origin)
        {
            Origin = origin;

            MaxHealth = new ReactiveProperty<int>(Origin.MaxHealth);
            Health = new ReactiveProperty<int>(Origin.Health);

            MaxHealth.Skip(1).Subscribe(e =>
            {
                origin.MaxHealth = origin.MaxHealth =Mathf.Clamp(e, 0, int.MaxValue);
                if (origin.Health > e) origin.Health = e;
            });

            Health.Skip(1).Subscribe(e => origin.Health = Mathf.Clamp(e, 0, origin.MaxHealth));
        }
    }
}