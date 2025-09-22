/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.
    
 
 using Microsoft.EntityFrameworkCore;
   using Microsoft.EntityFrameworkCore.Storage;
   using ScriptiumBackend.DB;
   using ScriptiumBackend.Models;
 
namespace ScriptiumBackend.Services;

public class UserDeletionBackgroundService(IServiceProvider serviceProvider, ILogger<UserDeletionBackgroundService> logger) : BackgroundService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    private readonly ILogger<UserDeletionBackgroundService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        const int periodOfCheck = -15;
        
        while (!stoppingToken.IsCancellationRequested)
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            DateTime thresholdDate = DateTime.UtcNow.AddDays(periodOfCheck);

            List<User> usersToDelete = await db.User
                .Where(u => u.DeletedAt != null && u.DeletedAt <= thresholdDate)
                .Include(u => u.Collections)
                .Include(u => u.Notes)
                .Include(u => u.Comments)
                .Include(u => u.Likes)
                .ToListAsync(stoppingToken);

            foreach (User user in usersToDelete)
            {
                await using IDbContextTransaction transaction = await db.Database.BeginTransactionAsync(stoppingToken);

                try
                {
                    db.Collection.RemoveRange(user.Collections);
                    db.Note.RemoveRange(user.Notes);
                    db.Comment.RemoveRange(user.Comments);
                    db.Like.RemoveRange(user.Likes);
                    //TODO: Add all
                    db.User.Remove(user);

                    await db.SaveChangesAsync(stoppingToken);
                    await transaction.CommitAsync(stoppingToken);

                    _logger.LogInformation($"User {user.Id} permanently deleted.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(stoppingToken);
                    _logger.LogError(ex, $"Error while deleting user {user.Id}. Changes rolled back.");
                }
            }

            await Task.Delay(TimeSpan.FromHours(12), stoppingToken);
        }
    }
}

*/