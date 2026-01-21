using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScriptiumBackend.Classes;
using ScriptiumBackend.Interfaces;
using ScriptiumBackend.Model.Util;

namespace ScriptiumBackend.Services.ServiceInterfaces;

public interface ICacheService
{
    Task<string?> GetPlain(string url);


    Task<Cache?> GetPlainCache(string url);


    Task<SerializedCache<T>?> Get<T>(string url); // where T : ICacheable; # Will be fixed


    Task<Cache> Save<T>(string url, T data, TimeSpan? validDuration = null)
        where T : ICacheable;

    Task<Cache> Save<T>(string url, List<T> data, TimeSpan? validDuration = null)
        where T : ICacheable;
}