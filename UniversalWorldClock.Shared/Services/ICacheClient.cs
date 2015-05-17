namespace UniversalWorldClock.Services
{
    public interface ICacheClient
    {
        T Get<T>(string key);
        void Set(string key, object data);
    }
}