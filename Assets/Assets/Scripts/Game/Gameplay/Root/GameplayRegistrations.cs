using AuroraWorld.App.Database;
using AuroraWorld.Gameplay.GameColony;
using AuroraWorld.Gameplay.GameplayTime;
using AuroraWorld.Gameplay.Player;
using AuroraWorld.Gameplay.World;
using DI;
using R3;
using UnityEngine;
using Time = AuroraWorld.Gameplay.GameplayTime.Time;

namespace AuroraWorld.Gameplay.Root
{
    public static class GameplayRegistrations
    {
        private static TimeProxy _timeProxy;
        private static WorldStateProxy _worldStateProxy;
        private static DIContainer _container;

        public static Observable<Unit> Register(DIContainer container, GameplayEnterParams enterParams)
        {
            _container = container;
            var geoSettings = Resources.Load<GeoConfiguration>("Configurations/Geo/Geo Settings");
            container.RegisterInstance(geoSettings);
            var storage = _container.Resolve<Storage>();

            var world = new GameObject("[WORLD]");
            container.RegisterInstance("ParentMeshTransform", world.transform);
            var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            Object.Destroy(plane.GetComponent<MeshCollider>());
            plane.transform.position = new Vector3(0, -5, 0);
            plane.transform.rotation = Quaternion.Euler(180, 0, 0);
            plane.transform.localScale = new Vector3(100, 100, 100);
            plane.transform.parent = world.transform;

            return Observable.Create<Unit>(observer =>
            {
                storage.Load($"{enterParams.WorldName}.GameTimeData", new Time())
                    .Subscribe(time =>
                    {
                        _timeProxy = new TimeProxy(_container, time);
                        _timeProxy.Hour.Skip(1).Subscribe(_ =>
                        {
                            var canAutoSave = _timeProxy.Hours % 2 == 0;
                            if(!canAutoSave) return;
                            Debug.Log("save..");

                            storage.Save($"{enterParams.WorldName}.GameTimeData", _timeProxy.Origin).Subscribe();
                            storage.Save($"{enterParams.WorldName}.WorldStateData", _worldStateProxy.Origin).Subscribe();

                        });
                        
                        storage.Load($"{enterParams.WorldName}.WorldStateData", new WorldState() { Seed = "sandbox", WorldEntityState = new WorldEntityState()})
                            .Subscribe(worldState =>
                            {
                                _worldStateProxy = new WorldStateProxy(_container, worldState, out var startPosition);

                                if (startPosition == Vector3Int.one)
                                {
                                    startPosition = new Vector3Int(
                                        PlayerPrefs.GetInt("cameraPositionX", 0),
                                        PlayerPrefs.GetInt("cameraPositionY", 0),
                                        PlayerPrefs.GetInt("cameraPositionZ", 0));
                                }
                                else
                                {
                                    PlayerPrefs.SetInt("cameraPositionX", startPosition.x);
                                    PlayerPrefs.SetInt("cameraPositionY", startPosition.y);
                                    PlayerPrefs.SetInt("cameraPositionZ", startPosition.z);
                                }
                                var user = new GameObject("[USER]").AddComponent<User>();
                                var camera = CameraRegister(user.transform, startPosition);
                                var userInput = new UserInput();
                                userInput.Run(_container, user, camera);
                                _container.RegisterInstance(userInput);
                                user.Run(_container, camera);
                                observer.OnNext(Unit.Default);
                                observer.OnCompleted();

                            });
                    });

                var population = new Population(new Pawn("Васек", new PawnLocation(Vector3Int.zero)));
                var colony = new Colony(population);
                _container.RegisterInstance(colony);

                return Disposable.Empty;
            });
        }

        private static Camera CameraRegister(Transform rootObject, Vector3 startPosition)
        {
            var cameraObject = new GameObject("[GAME CAMERA]")
            {
                transform = { parent = rootObject }
            };
            var gameplayCamera = cameraObject.AddComponent<Camera>();
            var cameraPosition = new Vector3(startPosition.x, 10, startPosition.z - 4);
            cameraObject.transform.position = cameraPosition;
            cameraObject.transform.rotation = Quaternion.LookRotation(startPosition - cameraPosition, Vector3.up);
            return gameplayCamera;
        }
    }
}