using System;
using Random = UnityEngine.Random;

namespace AuroraWorld.Gameplay.GamePawn
{
    [Serializable]
    public class Pawn
    {
        public PawnHealth Health;
        public PawnSkills Skills;

        public Pawn()
        {
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
                new Skill("Shooting", 0, Random.Range(0, 14), 0),
                new Skill("Melee", 0, Random.Range(0, 14), 0),
                new Skill("Construction", 0, Random.Range(0, 14), 0),
                new Skill("Mining", 0, Random.Range(0, 14), 0),
                new Skill("Cooking", 0, Random.Range(0, 14), 0),
                new Skill("Plants", 0, Random.Range(0, 14), 0),
                new Skill("Crafting", 0, Random.Range(0, 14), 0),
                new Skill("Artistic", 0, Random.Range(0, 14), 0),
                new Skill("Medicine", 0, Random.Range(0, 14), 0),
                new Skill("Social", 0, Random.Range(0, 14), 0),
                new Skill("Intellectual", 0, Random.Range(0, 14), 0)
            );
        }
    }
}