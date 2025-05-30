using AuroraWorld.Gameplay.Player;
using AuroraWorld.Gameplay.World;
using AuroraWorld.Gameplay.World.Geometry;
using DI;
using UnityEngine;

namespace AuroraWorld.Gameplay.Root
{
    public static class GameplayRegistrations
    {
        private static WorldStateProxy _worldState;
        private static DIContainer _container;

        public static void Register(DIContainer container, GameplayEnterParams enterParams)
        {
            _container = container;
            var geoSettings = Resources.Load<GeoConfiguration>("Configurations/Geo Settings");
            container.RegisterInstance(geoSettings);

            var world = new GameObject("[WORLD]");
            container.RegisterInstance("ParentMeshTransform", world.transform);


            WorldRegister(container, enterParams.WorldSeed);
            UserRegister();
        }

        private static void WorldRegister(DIContainer container, string seed)
        {
            var worldState = new WorldState();
            _worldState = new WorldStateProxy(container, worldState, seed);
            container.RegisterInstance(_worldState);
        }

        #region User Registers

        private static void UserRegister()
        {
            var user = new GameObject("[USER]").AddComponent<User>();
            var camera = CameraRegister(user.transform);
            
            var input =  new UserInput();
            input.Run(user, camera);
            _container.RegisterInstance(input);
            
            user.Run(_container);
        }

        private static Camera CameraRegister(Transform rootObject)
        {
            var cameraObject = new GameObject("[GAME CAMERA]");
            cameraObject.transform.parent = rootObject;
            var gameplayCamera = cameraObject.AddComponent<Camera>();
            cameraObject.transform.position = new Vector3(0, 10, -4);
            cameraObject.transform.rotation = Quaternion.LookRotation(-cameraObject.transform.position, Vector3.up);
            return gameplayCamera;
        }

        #endregion
    }
}