using System.IO;
using Newtonsoft.Json;
using R3;

namespace AuroraWorld.StorageGame
{
    public class WindowsStorageProvider : IStorageProvider
    {
        private const string FOLDER = "storages/";
        
        public Observable<bool> Save<T>(string tag, T obj)
        {
            return Observable.Create<bool>(observer =>
            {
                var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
                File.WriteAllText($"{FOLDER}{tag}.json", json);
                observer.OnNext(true);
                observer.OnCompleted();
                return Disposable.Empty;
            });
        }

        public Observable<T> Load<T>(string tag, T defaultObj = default)
        {
            return Observable.Create<T>(observer =>
            {
                if (!File.Exists($"{FOLDER}{tag}.json"))
                {
                    observer.OnNext(defaultObj);
                }
                else
                {
                    var json = File.ReadAllText($"{FOLDER}{tag}.json");
                    var loadedObj = JsonConvert.DeserializeObject<T>(json) ?? defaultObj;
                    observer.OnNext(loadedObj);
                }

                observer.OnCompleted();
                return Disposable.Empty;
            });
        }

        public Observable<bool> Reset<T>()
        {
            throw new System.NotImplementedException();
        }
    }
}