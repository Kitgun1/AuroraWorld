namespace AuroraWorld.Gameplay.GameColony
{
    public abstract class SubtaskProxy
    {
        public readonly SubtaskRequirements Requirements;
        public readonly Subtask Origin;

        protected SubtaskProxy(Subtask origin)
        {
            Origin = origin;
            Requirements = Origin.Requirements;
        }
        
        public float GetTaskWeight(Pawn pawn)
        {
            /*var pawnSkill = pawn.Skills.Skills.First(s => s.Type == Requirements.WorkType);
            if (Requirements.MinLevelSkill > pawnSkill.Level) return -1;

            int workPriority = Tasks.DefaultWorkPriority[Requirements.WorkType];
            var minDistance = Distance(TargetTerraform[0].CubePosition, pawn.Location.Position);
            minDistance = TargetTerraform
                .Select(terraformData => Distance(terraformData.CubePosition, pawn.Location.Position))
                .Prepend(minDistance).Min();

            return workPriority * 1.0f + pawnSkill.Interest * 0.7f + minDistance * 0.3f;

            int Distance(Vector3Int a, Vector3Int b) => CubeMath.Distance(a, b);*/
            return default;
        }
    }
}