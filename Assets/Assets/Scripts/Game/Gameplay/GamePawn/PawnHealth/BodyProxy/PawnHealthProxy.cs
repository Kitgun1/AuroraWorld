namespace AuroraWorld.Gameplay.GamePawn
{
    public class PawnHealthProxy
    {
        public readonly BodyPartProxy Head;
        public readonly BodyPartProxy LeftArm;
        public readonly BodyPartProxy RightArm;
        public readonly BodyPartProxy Torso;
        public readonly BodyPartProxy LeftLeg;
        public readonly BodyPartProxy RightLeg;

        public int Health => Head.Health + LeftArm.Health + RightArm.Health + Torso.Health + LeftLeg.Health +
                             RightLeg.Health;

        public int MaxHealth => Head.MaxHealth + LeftArm.MaxHealth + RightArm.MaxHealth + Torso.MaxHealth +
                                LeftLeg.MaxHealth + RightLeg.MaxHealth;

        public PawnHealthProxy(BodyPartProxy head, BodyPartProxy leftArm, BodyPartProxy rightArm, BodyPartProxy torso,
            BodyPartProxy leftLeg, BodyPartProxy rightLeg)
        {
            Head = head;
            LeftArm = leftArm;
            RightArm = rightArm;
            Torso = torso;
            LeftLeg = leftLeg;
            RightLeg = rightLeg;
        }
    }
}