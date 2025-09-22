
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ScriptiumBackend.Services;

public class IndexSyncBackgroundService(IServiceProvider provider, ILogger<IndexSyncBackgroundService> logger) : BackgroundService
{
    private readonly IServiceProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    private readonly ILogger<IndexSyncBackgroundService> _logger = logger ??  throw new ArgumentNullException(nameof(logger));
    private static readonly TimeSpan Interval = TimeSpan.FromDays(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _provider.CreateScope();
            var indexer = scope.ServiceProvider.GetRequiredService<LuceneIndexerService>();

            _logger.LogInformation("[IndexSync] Rebuilding Lucene index at {Time}", DateTime.UtcNow);
            await indexer.RebuildAllIndicesAsync();
            _logger.LogInformation("[IndexSync] Index rebuild completed at {Time}", DateTime.UtcNow);

            await Task.Delay(Interval, stoppingToken);
        }
    }
}