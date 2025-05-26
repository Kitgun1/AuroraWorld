using AuroraWorld.Gameplay.World;
using AuroraWorld.Gameplay.World.Geometry;
using DI;
using UnityEngine;

namespace AuroraWorld.Gameplay.Root
{
    public static class GameplayRegistrations
    {
        private static WorldStateProxy _worldState;

        public static void Register(DIContainer container, GameplayEnterParams enterParams)
        {
            var geoSettings = Resources.Load<GeoConfiguration>("Configurations/Geo Settings");
            container.RegisterInstance(geoSettings);
            
            var world = new GameObject("[WORLD]");
            container.RegisterInstance("ParentMeshTransform", world.transform);


            WorldRegister(container, enterParams.WorldSeed);
            CameraRegister();
        }

        private static void WorldRegister(DIContainer container, string seed)
        {
            var worldState = new WorldState();
            _worldState = new WorldStateProxy(container, worldState, seed);
        }

        private static void CameraRegister()
        {
            var camera = new GameObject("[GAME CAMERA]");
            camera.AddComponent<Camera>();
            camera.transform.position = new Vector3(0, 10, -4);
            camera.transform.rotation = Quaternion.LookRotation(Vector3.zero, Vector3.up);
        }
    }
}