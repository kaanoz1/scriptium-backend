using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using ScriptiumBackend.Interfaces;
using ScriptiumBackend.Services.ServiceInterfaces;

namespace ScriptiumBackend.Controllers.Islam.Hadith.ArabicOnly;

/// <summary>
/// Common response handling for the hadith endpoints: <c>{ data }</c>
/// envelope, URL-keyed caching (same scheme as the Qur'an controllers), 404 for
/// <c>null</c> and a logged 500 for unexpected errors.
/// </summary>
public abstract class HadithControllerBase(ICacheService cacheService, ILogger logger) : Controller
{
    /// <summary>Serves from / stores into the URL cache.</summary>
    protected async Task<IActionResult> Cached<T>(Func<Task<T?>> build, string? keySuffix = null,
        TimeSpan? validFor = null) where T : class, ICacheable
    {
        var cacheKey = Request.GetEncodedPathAndQuery() + (keySuffix ?? "");
        try
        {
            if (await cacheService.Get<T>(cacheKey) is { } cached)
                return Ok(new { data = cached.Data });

            var data = await build();
            if (data is null) return NotFound("Not found.");

            await cacheService.Save(cacheKey, data, validFor);
            return Ok(new { data });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Hadith endpoint failed: {Path}", cacheKey);
            return StatusCode(500, "Internal server error.");
        }
    }

    /// <summary>For paged / filtered lists, which would only bloat the cache.</summary>
    protected async Task<IActionResult> Uncached<T>(Func<Task<T?>> build) where T : class
    {
        try
        {
            var data = await build();
            if (data is null) return NotFound("Not found.");
            return Ok(new { data });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Hadith endpoint failed: {Path}", Request.GetEncodedPathAndQuery());
            return StatusCode(500, "Internal server error.");
        }
    }
}
