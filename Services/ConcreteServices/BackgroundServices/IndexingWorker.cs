using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Db;
using ScriptiumBackend.Interfaces;
using ScriptiumBackend.Services.ServiceInterfaces;

namespace ScriptiumBackend.Services.ConcreteServices.BackgroundServices;

public class IndexingWorker(IServiceProvider serviceProvider, ILogger<IndexingWorker> logger)
    : BackgroundService
{
    private readonly TimeSpan _period = TimeSpan.FromDays(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Indexing Worker started.");

        using var timer = new PeriodicTimer(_period);


        await ProcessEmbeddings(stoppingToken);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await ProcessEmbeddings(stoppingToken);
        }
    }

    private async Task ProcessEmbeddings(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting indexing cycle...");

        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ScriptiumDbContext>();
        var embeddingService = scope.ServiceProvider.GetRequiredService<IEmbeddingService>();

        const int batchSize = 50;


        var cutOffDate = DateTime.UtcNow.Subtract(_period);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var itemsToProcess = await dbContext.SearchableItems
                .Where(x => x.Embedding == null || x.LastEmbeddedAt < cutOffDate)
                .OrderBy(x => x.LastEmbeddedAt)
                .Take(batchSize)
                .ToListAsync(stoppingToken);

            if (itemsToProcess.Count == 0)
                break;


            foreach (var item in itemsToProcess.TakeWhile(_ => !stoppingToken.IsCancellationRequested))
            {
                if (item is ISearchableContent contentItem)
                {
                    var textToEmbed = contentItem.GetContentForEmbedding();

                    if (string.IsNullOrWhiteSpace(textToEmbed))
                    {
                        item.LastEmbeddedAt = DateTime.UtcNow;
                        continue;
                    }

                    var vector = await embeddingService.GenerateEmbeddingAsync(textToEmbed);

                    if (vector != null)
                    {
                        item.Embedding = vector;
                        item.LastEmbeddedAt = DateTime.UtcNow;
                        logger.LogInformation("Embedded Item ID: {Id}", item.Id);
                    }
                    else
                    {
                        logger.LogWarning("Failed to generate embedding for Item ID: {Id}. Skipping...", item.Id);
                        item.LastEmbeddedAt = DateTime.UtcNow;
                    }
                }
                else
                {
                    logger.LogWarning("Item {Id} does not implement ISearchableContent! Skipping to prevent loop...",
                        item.Id);

                    item.LastEmbeddedAt = DateTime.UtcNow;
                }
            }

            await dbContext.SaveChangesAsync(stoppingToken);

            await Task.Delay(100, stoppingToken);
        }

        logger.LogInformation("Indexing cycle completed.");
    }
}