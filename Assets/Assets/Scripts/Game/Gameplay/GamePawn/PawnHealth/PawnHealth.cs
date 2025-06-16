namespace AuroraWorld.Gameplay.GamePawn
{
    public class PawnHealth
    {
        public BodyPart Head;
        public BodyPart LeftArm;
        public BodyPart RightArm;
        public BodyPart Torso;
        public BodyPart LeftLeg;
        public BodyPart RightLeg;

        public PawnHealth(BodyPart head, BodyPart leftArm, BodyPart rightArm, BodyPart torso, BodyPart leftLeg,
            BodyPart rightLeg)
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