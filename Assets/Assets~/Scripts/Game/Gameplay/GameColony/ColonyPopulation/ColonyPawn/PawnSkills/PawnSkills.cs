using System;

namespace AuroraWorld.Gameplay.GameColony
{
    [Serializable]
    public class PawnSkills
    {
        public Skill[] Skills;

        public PawnSkills(params Skill[] skills)
        {
            Skills = skills;
        }
    }
}