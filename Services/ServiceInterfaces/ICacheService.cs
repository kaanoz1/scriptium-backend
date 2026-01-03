using ScriptiumBackend.Classes;
using ScriptiumBackend.Interfaces;
using ScriptiumBackend.Model.Util;

namespace ScriptiumBackend.Services.ServiceInterfaces;

public interface ICacheService
{
    /// <summary>
    /// Ham JSON verisini string olarak döner.
    /// </summary>
    Task<string?> GetPlain(string url);
    
    /// <summary>
    /// Ham CacheRecord kaydını döner.
    /// </summary>
    Task<Cache?> GetPlainCache(string url);

    /// <summary>
    /// Veriyi deserialize edilmiş obje olarak döner.
    /// </summary>
    Task<SerializedCache<T>?> Get<T>(string url); // where T : ICacheable; # Will be fixed

    /// <summary>
    /// Veriyi kaydeder veya günceller.
    /// validDuration: Verinin ne kadar süre 'taze' kabul edileceği. (Default: 10 gün)
    /// </summary>
    Task<Cache> Save<T>(string url, T data, TimeSpan? validDuration = null) 
        where T : ICacheable;

    Task<Cache> Save<T>(string url, List<T> data, TimeSpan? validDuration = null) 
        where T : ICacheable;
}