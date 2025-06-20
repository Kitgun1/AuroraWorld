namespace AuroraWorld.Gameplay.GameColony
{
    public class ColonyProxy
    {
        public PopulationProxy Population;
        public ColonyTasksProxy Tasks;
        
        public Colony Origin;

        public ColonyProxy(Colony origin)
        {
            Origin = origin;

            Population = new PopulationProxy(origin.Population);
            Tasks = new ColonyTasksProxy(origin.Tasks);
        }
    }
}