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
                var disposable = new CompositeDisposable();

                var completedCount = 0;
                var totalOperation = 2;
                
                storage.Load($"{enterParams.WorldName}.WorldStateData", new WorldState())
                    .Subscribe(worldState =>
                    {
                        _worldStateProxy = new WorldStateProxy(_container, worldState, out var startPosition);
                        
                        var user = new GameObject("[USER]").AddComponent<User>();
                        var camera = CameraRegister(user.transform, startPosition);
                        user.Run(_container, camera);

                        var userInput = new UserInput();
                        userInput.Run(_container, user, camera);
                        _container.RegisterInstance(userInput);
                        
                        if (++completedCount == totalOperation)
                        {
                            observer.OnNext(Unit.Default);
                            observer.OnCompleted();
                        }
                    })
                    .AddTo(disposable);
                
                storage.Load($"{enterParams.WorldName}.GameTimeData", new Time())
                    .Subscribe(time =>
                    {
                        _timeProxy = new TimeProxy(_container, time);
                        if (++completedCount == totalOperation)
                        {
                            observer.OnNext(Unit.Default);
                            observer.OnCompleted();
                        }
                    })
                    .AddTo(disposable);

                var population = new Population(new Pawn("Васек", new PawnLocation(Vector3Int.zero)));
                var colony = new Colony(population);
                _container.RegisterInstance(colony);
                
                return disposable;
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