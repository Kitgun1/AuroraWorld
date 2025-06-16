namespace AuroraWorld.Gameplay.GamePawn
{
    public class PawnProxy
    {
        public PawnHealthProxy Health;

        public readonly Pawn Origin;

        public PawnProxy(Pawn origin)
        {
            Origin = origin;

            Health = new PawnHealthProxy(
                new BodyPartProxy(Origin.Health.Head),
                new BodyPartProxy(Origin.Health.LeftArm),
                new BodyPartProxy(Origin.Health.RightArm),
                new BodyPartProxy(Origin.Health.Torso),
                new BodyPartProxy(Origin.Health.LeftLeg),
                new BodyPartProxy(Origin.Health.RightLeg)
                );
        }
    }
}