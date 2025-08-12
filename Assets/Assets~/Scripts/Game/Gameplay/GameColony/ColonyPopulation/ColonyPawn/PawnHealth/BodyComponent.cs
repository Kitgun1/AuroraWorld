using System;

namespace AuroraWorld.Gameplay.GameColony
{
    [Serializable]
    public class BodyComponent
    {
        public readonly string Name;
        public int MaxHealth;
        public int Health;

        public BodyComponent(string name, int initialHealth)
        {
            Name = name;
            MaxHealth = initialHealth;
            Health = initialHealth;
        }
    }
}