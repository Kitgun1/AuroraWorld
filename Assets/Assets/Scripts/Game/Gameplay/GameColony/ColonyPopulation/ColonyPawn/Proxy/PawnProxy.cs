using R3;

namespace AuroraWorld.Gameplay.GameColony
{
    public class PawnProxy
    {
        public ReactiveProperty<string> Name;
        public PawnLocationProxy Location;
        public PawnHealthProxy Health;
        public PawnSkillsProxy Skills;

        public readonly Pawn Origin;

        public PawnProxy(Pawn origin)
        {
            Origin = origin;

            Name = new ReactiveProperty<string>(origin.Name);
            Location = new PawnLocationProxy(Origin.Location);
            Health = new PawnHealthProxy(Origin.Health);
            Skills = new PawnSkillsProxy(Origin.Skills);

            Name.Skip(1).Subscribe(v => Origin.Name = v);
        }

        public override string ToString()
        {
            return $"{Name}. Position: {Location.Position} | HP: {Health.Health}\\{Health.MaxHealth}.\n" +
                   $"Skills:\n" +
                   $"{Skills}";
        }
    }
}