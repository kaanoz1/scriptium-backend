using ScriptiumBackend.Model.Util;

namespace ScriptiumBackend.Classes;

public class FetchedCache<T>(Cache cache, T data)
{
    public Cache Row { get; set; } = cache;
    public T Data { get; set; } = data;
}