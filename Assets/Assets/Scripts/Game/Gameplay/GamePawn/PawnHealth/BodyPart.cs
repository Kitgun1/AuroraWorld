using System;

namespace AuroraWorld.Gameplay.GamePawn
{
    [Serializable]
    public class BodyPart
    {
        public string Name { get; }
        public BodyComponent[] BodyComponents { get; }

        public BodyPart(string name, int health, params string[] bodyComponents)
        {
            Name = name;
            if (bodyComponents.Length == 0)
            {
                BodyComponents = new[] { new BodyComponent("EMPTY", health) };
                return;
            }

            BodyComponents = new BodyComponent[bodyComponents.Length];
            for (var i = 0; i < bodyComponents.Length; i++)
            {
                var bodyComponentName = bodyComponents[i];
                BodyComponents[i] = new BodyComponent(bodyComponentName, health / BodyComponents.Length);
            }
        }
    }
}