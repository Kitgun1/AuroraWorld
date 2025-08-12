using System;
using Random = UnityEngine.Random;

namespace AuroraWorld.Gameplay.GameColony
{
    [Serializable]
    public class Pawn
    {
        public string Name;
        public PawnLocation Location;
        public PawnHealth Health;

        public PawnSkills Skills;
        // TODO: Character Traits (Черты характера). Влияет на бонусы или приоритеты к выполнению задач
        // TODO: Needs (Потребности). Голод, жажда, сон, настроение.

        public Pawn(string name, PawnLocation location)
        {
            Name = name;
            Location = location;

            Health = new PawnHealth(
                new BodyPart("Head", 150, "Vision", "Hearing", "Speech"),
                new BodyPart("Left Arm", 60),
                new BodyPart("Right Arm", 60),
                new BodyPart("Torso", 200),
                new BodyPart("Left Leg", 65),
                new BodyPart("Right Leg", 65)
            );

            // TODO: Генерация параметров для скиллов
            Skills = new PawnSkills(
                new Skill(WorkType.Shooting, 0, Random.Range(0, 14), 0),
                new Skill(WorkType.Melee, 0, Random.Range(0, 14), 0),
                new Skill(WorkType.Construction, 0, Random.Range(0, 14), 0),
                new Skill(WorkType.Mining, 0, Random.Range(0, 14), 0),
                new Skill(WorkType.Cooking, 0, Random.Range(0, 14), 0),
                new Skill(WorkType.Plants, 0, Random.Range(0, 14), 0),
                new Skill(WorkType.Crafting, 0, Random.Range(0, 14), 0),
                new Skill(WorkType.Artistic, 0, Random.Range(0, 14), 0),
                new Skill(WorkType.Medicine, 0, Random.Range(0, 14), 0),
                new Skill(WorkType.Social, 0, Random.Range(0, 14), 0),
                new Skill(WorkType.Intellectual, 0, Random.Range(0, 14), 0)
            );
        }

        public override string ToString()
        {
            return $"{Name}. Position: {Location.Position}\n" +
                   $"Skills:\n{Skills}";
        }
    }
}