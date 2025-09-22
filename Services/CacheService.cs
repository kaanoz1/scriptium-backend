    using System.Text.Json;
    using Microsoft.EntityFrameworkCore;
    using ScriptiumBackend.Models;
    using ScriptiumBackend.Interface;
    using ScriptiumBackend.DB;
    using static Utility;

    namespace ScriptiumBackend.Services
    {
        public class CacheService(ApplicationDbContext db, ILogger<CacheService> logger) : ICacheService
        {
            private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
            private readonly ILogger<CacheService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            public async Task<T?> GetCachedDataAsync<T>(string key)
            {
                Cache? cache = await _db.Caches
                    .Where(c => c.Key == key && c.ExpirationDate > DateTime.UtcNow)
                    .FirstOrDefaultAsync();

                if (cache == null) return default;
                
                if (IsEffectivelyEmpty(cache))
                {
                    _logger.LogWarning($"Deserialized object of type {typeof(T).Name} is empty or invalid for key {key}");
                    return default;
                }

                try
                {
                    T? data = JsonSerializer.Deserialize<T>(cache.Data);

                    if (data is null)
                        return data;

                    var cacheR = new CacheRecord()
                    {
                        CacheId = cache.Id,
                        FetchedAt = DateTime.UtcNow
                    };

                    _db.CacheRecords.Add(cacheR);

                    await _db.SaveChangesAsync();

                    return data;
                }
                catch (Exception ex) when (ex is JsonException || ex is InvalidCastException)
                {
                    _logger.LogWarning($"Cache deserialization error for key {key}: {ex.Message}");
                    return default;
                }

            }


            public async Task SetCacheDataAsync<T>(string key, T data, TimeSpan? expirationTime = null)
            {
                string jsonData = JsonSerializer.Serialize(data);

                Cache? existingCacheEntry = await _db.Caches.FirstOrDefaultAsync(c => c.Key == key);

                if (existingCacheEntry != null)
                {
                    existingCacheEntry.Data = jsonData;
                    _db.Caches.Update(existingCacheEntry);
                }
                else
                {

                    Cache cacheEntry = new ()
                    {
                        Key = key,
                        Data = jsonData,
                        ExpirationDate = DateTime.UtcNow + (expirationTime ?? TimeSpan.FromDays(10))
                    };

                    _db.Caches.Add(cacheEntry);

                    CacheRecord cacheR = new()
                    {
                        Cache = cacheEntry,
                        FetchedAt = DateTime.UtcNow
                    };
                    _db.CacheRecords.Add(cacheR);
                }

                await _db.SaveChangesAsync();
            }


        }

    }
    
    