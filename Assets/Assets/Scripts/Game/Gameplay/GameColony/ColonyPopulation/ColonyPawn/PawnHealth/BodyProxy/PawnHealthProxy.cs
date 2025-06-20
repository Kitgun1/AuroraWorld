using System.Linq;

namespace AuroraWorld.Gameplay.GameColony
{
    public class PawnHealthProxy
    {
        public readonly BodyPartProxy[] BodyParts;

        public readonly PawnHealth Origin;
        
        public int Health => BodyParts.Sum(e => e.Health);
        public int MaxHealth => BodyParts.Sum(e => e.MaxHealth);
        
        public PawnHealthProxy(PawnHealth origin)
        {
            Origin = origin;
            
            BodyParts = new BodyPartProxy[Origin.BodyParts.Length];
            for (int i = 0; i < BodyParts.Length; i++)
            {
                BodyParts[i] = new BodyPartProxy(origin.BodyParts[i]);
            }
        }
    }
}