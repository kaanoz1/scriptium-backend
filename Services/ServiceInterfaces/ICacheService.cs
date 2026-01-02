using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Services.ServiceInterfaces;

public interface ICacheService
{
    /// <summary>
    /// Ham JSON verisini string olarak döner.
    /// </summary>
    string? GetPlain(string url);

    /// <summary>
    /// Veriyi deserialize edilmiş obje olarak döner.
    /// </summary>
    T? Get<T>(string url) where T : ICacheable;

    /// <summary>
    /// Veriyi kaydeder veya günceller.
    /// validDuration: Verinin ne kadar süre 'taze' kabul edileceği. (Default: 10 gün)
    /// </summary>
    void Save<T>(string url, T data, TimeSpan? validDuration = null) where T : ICacheable;
}