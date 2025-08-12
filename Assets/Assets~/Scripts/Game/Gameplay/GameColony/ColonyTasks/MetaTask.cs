using System;
using System.Collections.Generic;

namespace AuroraWorld.Gameplay.GameColony
{
    [Serializable]
    public class MetaTask
    {
        public readonly List<Subtask> Subtasks;
        public int WorkerLimit;

        public MetaTask(int workerLimit)
        {
            WorkerLimit = workerLimit;
            Subtasks = new List<Subtask>();
        }
    }
}