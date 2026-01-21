using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Classes;
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
        var cache = await _context.Caches
            .FirstOrDefaultAsync(c => c.Url == url);

        if (cache == null) return null;

        var record = new Util.CacheRecord
        {
            Cache = cache,
            FetchedAt = DateTime.UtcNow
        };

        await _context.Set<Util.CacheRecord>().AddAsync(record);
        await _context.SaveChangesAsync();

        return cache.Data;
    }

    public async Task<Util.Cache?> GetPlainCache(string url)
    {
        var cache = await _context.Caches
            .Include(c => c.Records)
            .FirstOrDefaultAsync(c => c.Url == url);

        if (cache is null)
            return null;

        var record = new Util.CacheRecord
        {
            Cache = cache,
            FetchedAt = DateTime.UtcNow
        };

        await _context.Set<Util.CacheRecord>().AddAsync(record);
        await _context.SaveChangesAsync();


        return cache;
    }

    public async Task<SerializedCache<T>?> Get<T>(string url)
    {
        var rawCache = await this.GetPlainCache(url);

        if (rawCache is null)
            return null;

        try
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var serialized = JsonSerializer.Deserialize<T>(rawCache.Data, options);

            ArgumentNullException.ThrowIfNull(serialized);

            var record = new Util.CacheRecord
            {
                Cache = rawCache,
                FetchedAt = DateTime.UtcNow
            };

            await _context.Set<Util.CacheRecord>().AddAsync(record);
            await _context.SaveChangesAsync();

            return new SerializedCache<T>(rawCache, serialized);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Cache Deserialization Error ({url}): {ex.Message}");
            return null;
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
            if ((now - existingCache.UpdatedAt) < duration)
            {
                return existingCache;
            }

            existingCache.Data = serializedData;
            existingCache.UpdatedAt = now;

            _context.Caches.Update(existingCache);
            resultCache = existingCache;
        }
        else
        {
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

    public async Task<Util.Cache> Save<T>(string url, List<T> data, TimeSpan? validDuration = null) where T : ICacheable
    {
        var duration = validDuration ?? TimeSpan.FromDays(10);

        var existingCache = await _context.Caches
            .FirstOrDefaultAsync(c => c.Url == url);

        var serializedData = JsonSerializer.Serialize(data);
        var now = DateTime.UtcNow;

        Util.Cache resultCache;

        if (existingCache != null)
        {
            if ((now - existingCache.UpdatedAt) < duration)
                return existingCache;


            existingCache.Data = serializedData;
            existingCache.UpdatedAt = now;

            _context.Caches.Update(existingCache);
            resultCache = existingCache;
        }
        else
        {
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