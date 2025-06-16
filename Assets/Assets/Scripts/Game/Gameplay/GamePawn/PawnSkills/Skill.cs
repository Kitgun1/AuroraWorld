using System;

namespace AuroraWorld.Gameplay.GamePawn
{
    [Serializable]
    public class Skill
    {
        public string Name;
        public int Interest;
        public int Level;
        public int Experience;

        public Skill(string name, int interest, int level, int experience)
        {
            Name = name;
            Interest = interest;
            Level = level;
            Experience = experience;
        }
    }
}