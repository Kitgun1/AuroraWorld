using R3;

namespace AuroraWorld.App.Database
{
    public class AkiCloudStorageProvider : IStorageProvider
    {
        public Observable<bool> Save<T>(string tag, T obj)
        {
            throw new System.NotImplementedException();
        }

        public Observable<bool> Reset<T>()
        {
            throw new System.NotImplementedException();
        }

        public Observable<T> Load<T>(string tag, T defaultObj = default)
        {
            throw new System.NotImplementedException();
        }
    }
}