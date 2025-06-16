using System;

namespace AuroraWorld.Gameplay.GamePawn
{
    [Serializable]
    public class PawnHealth
    {
        public BodyPart[] BodyParts;

        public PawnHealth(params BodyPart[] bodyParts)
        {
            BodyParts = bodyParts;
        }
    }
}