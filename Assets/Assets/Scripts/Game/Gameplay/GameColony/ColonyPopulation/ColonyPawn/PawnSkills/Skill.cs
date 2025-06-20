using System;

namespace AuroraWorld.Gameplay.GameColony
{
    [Serializable]
    public class Skill
    {
        public WorkType Type;
        public int Interest;
        public int Level;
        public int Experience;

        public Skill(WorkType type, int interest, int level, int experience)
        {
            Type = type;
            Interest = interest;
            Level = level;
            Experience = experience;
        }
    }
}