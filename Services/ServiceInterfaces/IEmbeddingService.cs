using Pgvector;

namespace ScriptiumBackend.Services.ServiceInterfaces;

public interface IEmbeddingService
{
    Task<Vector?> GenerateEmbeddingAsync(string text);
}