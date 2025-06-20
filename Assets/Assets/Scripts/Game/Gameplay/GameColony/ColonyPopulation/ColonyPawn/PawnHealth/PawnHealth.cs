using System;

namespace AuroraWorld.Gameplay.GameColony
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