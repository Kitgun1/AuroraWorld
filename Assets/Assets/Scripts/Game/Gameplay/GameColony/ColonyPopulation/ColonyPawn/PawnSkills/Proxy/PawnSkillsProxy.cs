namespace AuroraWorld.Gameplay.GameColony
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

        public override string ToString()
        {
            string result="";

            foreach (var skill in Skills)
            {
                result += $"{skill.Type}: interest - {skill.Interest} ({skill.Level} lvl, {skill.Experience} exp)\n";
            }
            
            return result;
        }
    }
}