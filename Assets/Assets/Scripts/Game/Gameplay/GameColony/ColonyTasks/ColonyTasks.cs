using System;
using System.Collections.Generic;

namespace AuroraWorld.Gameplay.GameColony
{
    [Serializable]
    public class ColonyTasks
    {
        public List<MetaTask> MetaTasks;

        public ColonyTasks()
        {
            MetaTasks = new List<MetaTask>();
        }
    }
}