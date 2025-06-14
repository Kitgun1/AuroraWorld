using System.IO;
using Newtonsoft.Json;
using R3;
using UnityEngine;

namespace AuroraWorld.App.Database
{
    public class WindowsStorageProvider : IStorageProvider
    {
        private const string FOLDER = "storages/";
        
        public Observable<bool> Save<T>(string tag, T obj)
        {
            return Observable.Create<bool>(observer =>
            {
                var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
                var folderPath = Path.Combine(Application.persistentDataPath, $"{FOLDER}");
                var filePath = Path.Combine(folderPath, $"{tag}.json");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                File.WriteAllText(filePath, json);
                observer.OnNext(true);
                observer.OnCompleted();
                return Disposable.Empty;
            });
        }

        public Observable<T> Load<T>(string tag, T defaultObj = default)
        {
            return Observable.Create<T>(observer =>
            {
                var path = Path.Combine(Application.persistentDataPath, $"{FOLDER}{tag}.json");
                if (!File.Exists(path))
                {
                    observer.OnNext(defaultObj);
                }
                else
                {
                    var json = File.ReadAllText(path);
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