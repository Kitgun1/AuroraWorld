using R3;

namespace AuroraWorld.App.Database
{
    public class Storage
    {
        private readonly IStorageProvider _storage;

        public Storage()
        {
#if UNITY_EDITOR
            _storage = new WindowsStorageProvider();
            return;
#endif
            // TODO: storage for server, yandex game server
        }

        public Observable<bool> Save<T>(string tag, T obj) => _storage.Save(tag, obj);
        public Observable<T> Load<T>(string tag, T defaultObj = default) => _storage.Load(tag, defaultObj);
    }
}