namespace AuroraWorld.Gameplay.GamePawn
{
    public class PawnSkillsProxy
    {
        public SkillProxy[] Skills;

        public readonly PawnSkills Origin;

        public PawnSkillsProxy(PawnSkills origin)
        {
            Origin = origin;
            
            Skills = new SkillProxy[Origin.Skills.Length];
            for (int i = 0; i < Skills.Length; i++)
            {
                Skills[i] = new SkillProxy(origin.Skills[i]);
            }
        }
    }
}