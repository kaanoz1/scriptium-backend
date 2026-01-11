using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Db;
using ScriptiumBackend.Dto.Derived.Islam.Quranic.Root;
using ScriptiumBackend.Services.ServiceInterfaces;

namespace ScriptiumBackend.Controllers.Islam.Quran;

[ApiController, Route("api/islam/quranic")]
public class RootController(
    ScriptiumDbContext context,
    ILogger<VerseController> logger,
    ICacheService cacheService)
    : Controller
{
    private readonly ScriptiumDbContext _context = context;
    private readonly ILogger<VerseController> _logger = logger;
    private readonly ICacheService _cacheService = cacheService;


    [HttpGet("/root/{latin}")]
    public async Task<IActionResult> Get([FromRoute, StringLength(5, MinimumLength = 3), Required] string latin)
    {
        //TODO: Add validation.

        var cacheKey = Request.GetEncodedPathAndQuery();

        try
        {
            if (await _cacheService.Get<UpToVerse>(cacheKey) is { } serializedCache)
            {
                _logger.LogInformation("Cache has been found. Cache.Id: {Id}. Sending.", serializedCache.Raw.Id);
                return Ok(new { data = serializedCache.Data });
            }


            var root = await _context.QRoots
                .AsNoTracking()
                .AsSplitQuery()
                .Include(r => r.Words)
                .Include(r => r.Words).ThenInclude(w => w.Transliterations).ThenInclude(t => t.Language)
                .Include(r => r.Words).ThenInclude(w => w.Meanings).ThenInclude(m => m.Language)
                .Include(r => r.Words).ThenInclude(w => w.Verse)
                .FirstOrDefaultAsync(r => r.TextInLatin == latin);

            if (root is null)
                return NotFound("No root found.");

            var data = root.ToUpToVerseDto();

            var savedCacheRow = await _cacheService.Save(cacheKey, data);

            _logger.LogInformation("Cache saved. Model name: Verse. Cache.Id: {CacheId}", savedCacheRow.Id);

            return Ok(new { data });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "An unexpected error occurred while processing root. Latin: {Latin}",
                latin);

            return StatusCode(500, "Internal server error. Please try again later.");
        }
    }
}