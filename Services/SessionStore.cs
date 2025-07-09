using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using scriptium_backend_dotnet.DB;
using scriptium_backend_dotnet.Models;

namespace scriptium_backend_dotnet.Services
{
    public class SessionStore : ITicketStore
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SessionStore> _logger;
        private readonly TimeSpan _ticketExpiration = TimeSpan.FromDays(3);

        public SessionStore(IServiceProvider serviceProvider, ILogger<SessionStore> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            _logger.LogInformation("StoreAsync called to create a new session.");

            using var scope = _serviceProvider.CreateScope();
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                var userIdClaim = ticket.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    _logger.LogError("StoreAsync failed: UserId claim not found in the authentication ticket.");
                    throw new InvalidOperationException("UserId claim not found.");
                }

                if (!Guid.TryParse(userIdClaim, out Guid userId))
                {
                    _logger.LogError($"StoreAsync failed: Invalid UserId format '{userIdClaim}'.");
                    throw new InvalidOperationException("Invalid UserId format.");
                }

                var sessionId = Guid.NewGuid().ToString();
                _logger.LogInformation($"Generated new SessionId: {sessionId} for UserId: {userId}.");

                var session = new Session
                {
                    Key = sessionId,
                    UserId = userId,
                    ExpiresAt = DateTime.UtcNow.Add(_ticketExpiration)
                };

                try
                {
                    db.Session.Add(session);
                    await db.SaveChangesAsync();
                    _logger.LogInformation($"Session with SessionId: {sessionId} successfully stored in the database.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "StoreAsync encountered an error while saving the session to the database.");
                    throw;
                }

                return sessionId;
            }
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            _logger.LogInformation($"RenewAsync called for SessionId: {key}.");

            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                try
                {
                    var session = await db.Session.FirstOrDefaultAsync(s => s.Key == key);
                    if (session == null)
                    {
                        _logger.LogWarning($"RenewAsync: SessionId: {key} not found.");
                        return;
                    }

                    if (session.ExpiresAt < DateTime.UtcNow)
                    {
                        _logger.LogWarning($"RenewAsync: SessionId: {key} has already expired at {session.ExpiresAt}.");
                        return;
                    }

                    session.ExpiresAt = DateTime.UtcNow.Add(_ticketExpiration);
                    db.Session.Update(session);
                    await db.SaveChangesAsync();

                    _logger.LogInformation($"RenewAsync: SessionId: {key} expiration updated to {session.ExpiresAt}.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"RenewAsync encountered an error while renewing the session with SessionId: {key}.");
                    throw;
                }
            }
        }

        public async Task<AuthenticationTicket?> RetrieveAsync(string key)
        {
            _logger.LogInformation($"RetrieveAsync called for SessionId: {key}.");

            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                try
                {
                    var session = await db.Session.FirstOrDefaultAsync(s => s.Key == key);

                    if (session == null)
                    {
                        _logger.LogWarning($"RetrieveAsync: SessionId: {key} not found.");
                        return null;
                    }

                    if (session.ExpiresAt < DateTime.UtcNow)
                    {
                        _logger.LogWarning($"RetrieveAsync: SessionId: {key} has expired at {session.ExpiresAt}.");
                        return null;
                    }

                    User? user = await db.Users.FindAsync(session.UserId);

                    if (user == null)
                    {
                        _logger.LogWarning($"RetrieveAsync: UserId: {session.UserId} associated with SessionId: {key} not found.");
                        return null;
                    }

                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new(ClaimTypes.Name, user.Id.ToString()),
                        new(ClaimTypes.UserData, user.UserName ?? "\\0"),
                    };

                    var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    var props = new AuthenticationProperties
                    {
                        ExpiresUtc = session.ExpiresAt,
                        IsPersistent = true,
                        AllowRefresh = true,
                    };

                    var ticket = new AuthenticationTicket(principal, props, IdentityConstants.ApplicationScheme);
                    _logger.LogInformation($"RetrieveAsync: Authentication ticket created for SessionId: {key}.");

                    user.LastActive = DateTime.UtcNow;
                    await db.SaveChangesAsync();
                    _logger.LogInformation($"User: [Id: {user.Id}, Username: {user.UserName}]'s last active property has been updated.");

                    return ticket;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"RetrieveAsync encountered an error while retrieving the session with SessionId: {key}.");
                    throw;
                }
            }
        }

        public async Task RemoveAsync(string key)
        {
            _logger.LogInformation($"RemoveAsync called for SessionId: {key}.");

            using (var scope = _serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

                try
                {
                    var session = await db.Session.FirstOrDefaultAsync(s => s.Key == key);

                    if (session == null)
                    {
                        _logger.LogWarning($"RemoveAsync: SessionId: {key} not found.");
                        return;
                    }

                    db.Session.Remove(session);
                    await db.SaveChangesAsync();

                    _logger.LogInformation($"RemoveAsync: SessionId: {key} successfully removed from the database.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"RemoveAsync encountered an error while removing the session with SessionId: {key}.");
                    throw;
                }
            }
        }
    }
}
