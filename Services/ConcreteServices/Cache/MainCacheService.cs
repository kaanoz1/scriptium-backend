using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Db;
using ScriptiumBackend.Interfaces;
using ScriptiumBackend.Services.ServiceInterfaces;
using Util = ScriptiumBackend.Model.Util;

namespace ScriptiumBackend.Services.ConcreteServices.Cache;

public class MainCacheService(ScriptiumDbContext context) : ICacheService
{
    private readonly ScriptiumDbContext _context = context;

    public async Task<string?> GetPlain(string url)
    {
        // 1. Veriyi bul (Async)
        var cache = await _context.Caches
            .FirstOrDefaultAsync(c => c.Url == url);

        if (cache == null) return null;

        // 2. Erişim kaydı (Log) oluştur
        var record = new Util.CacheRecord
        {
            Cache = cache, // İlişki ID üzerinden veya obje üzerinden kurulabilir
            FetchedAt = DateTime.UtcNow
        };

        // Log yazma işlemini de async yapıyoruz
        await _context.Set<Util.CacheRecord>().AddAsync(record);
        await _context.SaveChangesAsync();

        return cache.Data;
    }

    // Interface'de eksik olan GetCache metodu
    public async Task<Util.Cache?> GetCache(string url)
    {
         return await _context.Caches
            .Include(c => c.Records) // Gerekirse ilişkileri de getir
            .FirstOrDefaultAsync(c => c.Url == url);
    }

    // Generic Get metodu (Bunun Interface'de Task<T?> olması gerekir)
    public async Task<T?> Get<T>(string url) where T : ICacheable
    {
        var rawData = await GetPlain(url);

        if (string.IsNullOrEmpty(rawData)) 
            return default;

        try 
        {
            return JsonSerializer.Deserialize<T>(rawData);
        }
        catch 
        {
            return default;
        }
    }

    public async Task<Util.Cache> Save<T>(string url, T data, TimeSpan? validDuration = null) where T : ICacheable
    {
        var duration = validDuration ?? TimeSpan.FromDays(10);
        
        var existingCache = await _context.Caches
            .FirstOrDefaultAsync(c => c.Url == url);

        var serializedData = JsonSerializer.Serialize(data);
        var now = DateTime.UtcNow;

        Util.Cache resultCache;

        if (existingCache != null)
        {
            // Süre kontrolü
            if ((now - existingCache.UpdatedAt) < duration)
            {
                // Veri hala taze, güncellemeye gerek yok, mevcut olanı dön
                // Ancak veriyi güncellemek istersen bu if bloğunu kaldırabilirsin.
                return existingCache; 
            }

            existingCache.Data = serializedData;
            existingCache.UpdatedAt = now;
            
            _context.Caches.Update(existingCache);
            resultCache = existingCache;
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

            await _context.Caches.AddAsync(newCache);
            resultCache = newCache;
        }

        await _context.SaveChangesAsync();
        return resultCache;
    }
}