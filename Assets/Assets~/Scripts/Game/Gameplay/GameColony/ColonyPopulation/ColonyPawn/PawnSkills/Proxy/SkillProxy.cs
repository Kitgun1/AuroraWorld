using R3;
using UnityEngine;

namespace AuroraWorld.Gameplay.GameColony
{
    public class SkillProxy
    {
        public WorkType Type => Origin.Type;
        public int Interest => Origin.Interest;
        public readonly ReactiveProperty<int> Level;
        public readonly ReactiveProperty<int> Experience;

        public readonly Skill Origin;

        public SkillProxy(Skill origin)
        {
            Origin = origin;

            Level = new ReactiveProperty<int>(Origin.Level);
            Experience = new ReactiveProperty<int>(Origin.Experience);

            Level.Skip(1).Subscribe(e => Origin.Level = Mathf.Clamp(e, 0, 20));
            Experience.Skip(1).Subscribe(e => Origin.Experience = Mathf.Clamp(e, 0, GetExpMax(Level.Value)));
        }

        public static int GetExpMax(int level) => 1 + level / 5 + 1000;
    }
}