using System.Linq;

namespace AuroraWorld.Gameplay.GameColony
{
    public class BodyPartProxy
    {
        public string Name => Origin.Name;
        public readonly BodyComponentProxy[] BodyComponents;

        public readonly BodyPart Origin;

        public int Health => BodyComponents.Sum(e => e.Health.Value);
        public int MaxHealth => BodyComponents.Sum(e => e.MaxHealth.Value);

        public BodyPartProxy(BodyPart origin)
        {
            Origin = origin;

            BodyComponents = new BodyComponentProxy[Origin.BodyComponents.Length];
            for (int i = 0; i < BodyComponents.Length; i++)
            {
                BodyComponents[i] = new BodyComponentProxy(origin.BodyComponents[i]);
            }
        }
    }
}