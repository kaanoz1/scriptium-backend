using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Pgvector;
using ScriptiumBackend.Services.ServiceInterfaces;

namespace ScriptiumBackend.Services.ConcreteServices.Embedding;

public class OllamaEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OllamaEmbeddingService> _logger;
    private const string ModelName = "nomic-embed-text";

    public OllamaEmbeddingService(HttpClient httpClient, ILogger<OllamaEmbeddingService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        
        var baseUrl = configuration["OLLAMA_BASE_URL"] ?? "http://localhost:11434";
        _httpClient.BaseAddress = new Uri(baseUrl);
    }

    public async Task<Vector?> GenerateEmbeddingAsync(string text)
    {
        try
        {
            var request = new
            {
                model = ModelName,
                prompt = text
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/api/embeddings", jsonContent);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OllamaResponse>(responseString);

            return new Vector(result?.Embedding);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ollama embedding generation failed for text: {TextSnippet}...", text[..Math.Min(text.Length, 20)]);
            return null;
        }
    }

    private class OllamaResponse
    {
        [JsonPropertyName("embedding")]
        public float[]? Embedding { get; set; }
    }
}