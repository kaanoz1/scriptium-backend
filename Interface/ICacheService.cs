namespace ScriptiumBackend.Interface;

public interface ICacheService
{
    Task<T?> GetCachedDataAsync<T>(string key);
    Task SetCacheDataAsync<T>(string key, T data, TimeSpan? expirationTime = null);
}