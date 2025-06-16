using System;

namespace AuroraWorld.Gameplay.GamePawn
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