namespace AuroraWorld.Gameplay.GamePawn
{
    public class PawnProxy
    {
        public PawnHealthProxy Health;
        public PawnSkillsProxy Skills;

        public readonly Pawn Origin;

        public PawnProxy(Pawn origin)
        {
            Origin = origin;
            
            Health = new PawnHealthProxy(Origin.Health);
            Skills = new PawnSkillsProxy(Origin.Skills);
        }
    }
}