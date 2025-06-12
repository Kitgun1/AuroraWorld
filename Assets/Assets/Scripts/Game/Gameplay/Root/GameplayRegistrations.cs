using AuroraWorld.Gameplay.GameplayTime;
using AuroraWorld.Gameplay.Player;
using AuroraWorld.Gameplay.World;
using AuroraWorld.Gameplay.World.Geometry;
using DI;
using UnityEngine;
using Time = AuroraWorld.Gameplay.GameplayTime.Time;

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


            TimeRegister();
            var startPosition = WorldRegister(container, enterParams.WorldSeed);
            UserRegister(startPosition.CubeToWorld());
        }

        private static Vector3Int WorldRegister(DIContainer container, string seed)
        {
            var worldState = new WorldState();
            _worldState = new WorldStateProxy(container, worldState, seed, out var startPosition);
            container.RegisterInstance(_worldState);
            return startPosition;
        }

        private static void TimeRegister()
        {
            var time = new Time(0);
            var timeProxy = new TimeProxy(time);
            _container.RegisterInstance(timeProxy);
        }

        #region User Registers

        private static void UserRegister(Vector3 startPosition)
        {
            var user = new GameObject("[USER]").AddComponent<User>();
            var camera = CameraRegister(user.transform, startPosition);

            var input = new UserInput();
            input.Run(user, camera);
            _container.RegisterInstance(input);

            user.Run(_container);
        }

        private static Camera CameraRegister(Transform rootObject, Vector3 startPosition)
        {
            var cameraObject = new GameObject("[GAME CAMERA]");
            cameraObject.transform.parent = rootObject;
            var gameplayCamera = cameraObject.AddComponent<Camera>();
            cameraObject.transform.position = new Vector3(startPosition.x, 10, startPosition.z - 4);
            cameraObject.transform.rotation =
                Quaternion.LookRotation(startPosition - cameraObject.transform.position, Vector3.up);
            return gameplayCamera;
        }

        #endregion
    }
}