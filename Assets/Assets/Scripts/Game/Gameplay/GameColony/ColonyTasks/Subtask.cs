using System;

namespace AuroraWorld.Gameplay.GameColony
{
    [Serializable]
    public abstract class Subtask
    {
        public SubtaskRequirements Requirements;

        protected Subtask(WorkType workType, int minLevel)
        {
            Requirements = new SubtaskRequirements { WorkType = workType, MinLevel = minLevel };
        }
    }
}