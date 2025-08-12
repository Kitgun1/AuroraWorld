using System;

namespace AuroraWorld.Gameplay.GameColony
{
    [Serializable]
    public class Colony
    {
        public Population Population;
        public ColonyTasks Tasks;

        public Colony(Population population)
        {
            Population = population;
            Tasks = new ColonyTasks();
        }

        /*public void DistributeTasks()
        {
            var workingPawns = Tasks.GetAllWorkers();
            var freePawns = Population.Pawns.Where(p => !workingPawns.Contains(p)).ToArray();

            // Сначала распределяем задачи, в которых мало колонистов
            var tasks = Tasks.GetAvailableTasks().OrderByDescending(t => t.Pawns.Count(p => p == null)).ToList();
            while (freePawns.Length > 0 && tasks.Count > 0)
            {
                foreach (var availableTask in tasks)
                {
                    var availablePawns = freePawns
                        .Where(p => p.Skills.Skills.Any(s =>
                            s.Type == availableTask.Requirements.WorkType &&
                            s.Level >= availableTask.Requirements.MinLevelSkill)).ToArray();
                    var bestPawn = availablePawns
                        .Where(p => (int)availableTask.GetTaskWeight(p) != -1)
                        .OrderByDescending(p => availableTask.GetTaskWeight(p)).FirstOrDefault();
                    availableTask.SetPawn(bestPawn);
                    if (availableTask.CountAvailableJobs() == 0) tasks.Remove(availableTask);
                    break;
                }
                freePawns = Population.Pawns.Where(p => !workingPawns.Contains(p)).ToArray();
            }
        }*/
    }
}