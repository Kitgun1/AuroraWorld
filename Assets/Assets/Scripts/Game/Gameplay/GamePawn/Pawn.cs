using System;

namespace AuroraWorld.Gameplay.GamePawn
{
    [Serializable]
    public class Pawn
    {
        public PawnHealth Health;

        public Pawn()
        {
            Health = new PawnHealth(
                new BodyPart("Head", 150, "Vision", "Hearing", "Speech"),
                new BodyPart("Left Arm", 60),
                new BodyPart("Right Arm", 60),
                new BodyPart("Torso", 200),
                new BodyPart("Left Leg", 65),
                new BodyPart("Right Leg", 65)
            );
        }
    }
}