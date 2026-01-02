using System.Text.Json;
using ScriptiumBackend.Db;
using ScriptiumBackend.Interfaces;
using Util = ScriptiumBackend.Model.Util;

namespace ScriptiumBackend.Services.ConcreteServices.Cache;

public class MainCacheService(ScriptiumDbContext context)
{
    private readonly ScriptiumDbContext _context = context;
    public string? GetPlain(string url)
    {
        // 1. Veriyi bul
        // Not: DbSet<Cache> tanımlı değilse Set<Cache>() kullanırız.
        var cache = _context.Caches
            .FirstOrDefault(c => c.Url == url);

        if (cache == null) return null;

        // 2. Erişim kaydı (Log) oluştur
        var record = new Util.CacheRecord
        {
            Cache = cache,
            FetchedAt = DateTime.UtcNow
        };

        _context.Set<Util.CacheRecord>().Add(record);
        _context.SaveChanges();

        return cache.Data;
    }

    public T? Get<T>(string url) where T : ICacheable
    {
        var rawData = GetPlain(url);

        if (string.IsNullOrEmpty(rawData)) 
            return null;

        try 
        {
            return JsonSerializer.Deserialize<T>(rawData);
        }
        catch 
        {
            // Loglama mekanizman varsa buraya ekleyebilirsin
            return null;
        }
    }

    public void Save<T>(string url, T data, TimeSpan? validDuration = null) where T : ICacheable
    {
        var duration = validDuration ?? TimeSpan.FromDays(10);
        
        var existingCache = _context.Caches
            .FirstOrDefault(c => c.Url == url);

        var serializedData = JsonSerializer.Serialize(data);
        var now = DateTime.UtcNow;

        if (existingCache != null)
        {
            if ((now - existingCache.UpdatedAt) < duration)
            {
                return;
            }

            existingCache.Data = serializedData;
            existingCache.UpdatedAt = now;
            
            _context.Set<Util.Cache>().Update(existingCache);
        }
        else
        {
            // Yeni kayıt
            var newCache = new Util.Cache
            {
                Url = url,
                Data = serializedData,
                CreatedAt = now,
                UpdatedAt = now,
                Records = []
            };

            _context.Set<Util.Cache>().Add(newCache);
        }

        _context.SaveChanges();
    }
}