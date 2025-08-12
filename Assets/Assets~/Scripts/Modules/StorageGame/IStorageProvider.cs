using R3;

namespace AuroraWorld.App.Database
{
    public interface IStorageProvider
    {
        public Observable<T> Load<T>(string tag, T defaultObj = default);
        public Observable<bool> Save<T>(string tag, T obj);
        public Observable<bool> Reset<T>();
    }
}