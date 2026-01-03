using ScriptiumBackend.Model.Util;

namespace ScriptiumBackend.Classes;

public class SerializedCache<T>(Cache cache, T data)
{
    public Cache Raw { get; set; } = cache;
    public T Data { get; set; } = data;
}