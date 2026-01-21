using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pgvector.EntityFrameworkCore;
using ScriptiumBackend.Classes.ResponseClasses.SearchResult;
using ScriptiumBackend.Db;
using ScriptiumBackend.Dto.Derived.Islam.Quranic.VerseTranslation; 
using ScriptiumBackend.Services.ServiceInterfaces;

namespace ScriptiumBackend.Controllers.Islam.Quran;

[ApiController, Route("api/islam/quranic")]
public class SearchController(
    ScriptiumDbContext db,
    ILogger<SearchController> logger,
    ICacheService cacheService,
    IEmbeddingService embeddingService)
    : Controller
{
    private const int ResultCount = 20;

    [HttpGet("context-search")]
    public async Task<IActionResult> Search(
        [FromQuery, Required, StringLength(100, MinimumLength = 2), RegularExpression(@"^[a-zA-Z0-9\s\.,'?!-]+$",
             ErrorMessage = "Only English characters, numbers, and basic punctuation are allowed.")]
        string query)
    {
        var cacheKey = Request.GetEncodedPathAndQuery();

        try
        {
            if (await cacheService.Get<SearchResult>(cacheKey) is { } serializedCache)
            {
                logger.LogInformation("Cache has been found. Cache.Id: {Id}. Sending.", serializedCache.Raw.Id);
                return Ok(new { data = serializedCache.Data });
            }

            var queryEmbedding = await embeddingService.GenerateEmbeddingAsync(query);
            if (queryEmbedding is null) return BadRequest("Embedding generation failed.");

            var orderedTranslationIds = await db.QVerseTranslations
                .AsNoTracking()
                .Where(vt => vt.Embedding != null)
                .OrderBy(vt => vt.Embedding!.L2Distance(queryEmbedding))
                .Select(vt => vt.Id)
                .Take(ResultCount)
                .ToListAsync();

            if (!orderedTranslationIds.Any())
                return Ok(new { data = new SearchResult(new List<WithVerse>()) });

            var translationsUnordered = await db.QVerseTranslations
                .AsNoTrackingWithIdentityResolution() 
                .Where(vt => orderedTranslationIds.Contains(vt.Id))
                .AsSplitQuery()
                
                .Include(vt => vt.Footnotes)
                .Include(vt => vt.Translation).ThenInclude(t => t.Language)
                .Include(vt => vt.Translation).ThenInclude(t => t.Authors).ThenInclude(a => a.Language)
                .Include(vt => vt.Translation).ThenInclude(t => t.Authors).ThenInclude(a => a.NameTranslations).ThenInclude(nt => nt.Language)
                
                .Include(vt => vt.Verse)
                    .ThenInclude(v => v.Chapter).ThenInclude(c => c.Meanings).ThenInclude(m => m.Language)
                
                .Include(vt => vt.Verse)
                    .ThenInclude(v => v.Transliterations).ThenInclude(t => t.Language)
                
                .Include(vt => vt.Verse)
                    .ThenInclude(v => v.Translations).ThenInclude(t => t.Footnotes)
                .Include(vt => vt.Verse)
                    .ThenInclude(v => v.Translations).ThenInclude(t => t.Language)
                .Include(vt => vt.Verse)
                    .ThenInclude(v => v.Translations).ThenInclude(t => t.Translation).ThenInclude(t => t.Language)
                .Include(vt => vt.Verse)
                    .ThenInclude(v => v.Translations).ThenInclude(t => t.Translation).ThenInclude(t => t.Authors).ThenInclude(a => a.Language)
                .Include(vt => vt.Verse)
                    .ThenInclude(v => v.Translations).ThenInclude(t => t.Translation).ThenInclude(t => t.Authors).ThenInclude(a => a.NameTranslations).ThenInclude(nt => nt.Language)
                
                .ToListAsync();

            var orderedTranslations = orderedTranslationIds
                .Select(id => translationsUnordered.FirstOrDefault(vt => vt.Id == id))
                .Where(vt => vt != null)
                .Select(vt => vt!.ToWithVerse())
                .ToList();

            var result = new SearchResult(orderedTranslations);

            await cacheService.Save(cacheKey, result, TimeSpan.FromDays(30));

            return Ok(new { data = result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in search. Query: {Query}", query);
            return StatusCode(500, "Internal server error.");
        }
    }

   [HttpGet("text-search")]
    public async Task<IActionResult> TextSearch(
        [FromQuery, Required, StringLength(100, MinimumLength = 2),
         RegularExpression(@"^[a-zA-Z0-9\s\.,'?!-]+$",
             ErrorMessage = "Only English characters, numbers, and basic punctuation are allowed.")
        ]
        string query)
    {
        var cacheKey = Request.GetEncodedPathAndQuery();

        try
        {
            if (await cacheService.Get<SearchResult>(cacheKey) is { } serializedCache)
            {
                logger.LogInformation("Cache has been found. Cache.Id: {Id}. Sending.", serializedCache.Raw.Id);
                return Ok(new { data = serializedCache.Data });
            }

            var searchTerms = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var initialQuery = db.QVerseTranslations.AsNoTrackingWithIdentityResolution();

            foreach (var term in searchTerms)
            {
                initialQuery = initialQuery.Where(vt => EF.Functions.ILike(vt.Text, $"%{term}%"));
            }

            var orderedTranslationIds = await initialQuery
                .OrderByDescending(vt => EF.Functions.TrigramsSimilarity(vt.Text, query))
                .Select(vt => vt.Id)
                .Take(ResultCount)
                .ToListAsync();

            if (!orderedTranslationIds.Any())
                return Ok(new { data = new SearchResult(new List<WithVerse>()) });

            var translationsUnordered = await db.QVerseTranslations
                .AsNoTrackingWithIdentityResolution()
                .Where(vt => orderedTranslationIds.Contains(vt.Id))
                .AsSplitQuery()
                
                .Include(vt => vt.Footnotes)
                .Include(vt => vt.Translation).ThenInclude(t => t.Language)
                .Include(vt => vt.Translation).ThenInclude(t => t.Authors).ThenInclude(a => a.Language)
                .Include(vt => vt.Translation).ThenInclude(t => t.Authors).ThenInclude(a => a.NameTranslations).ThenInclude(nt => nt.Language)
                
                .Include(vt => vt.Verse)
                    .ThenInclude(v => v.Chapter).ThenInclude(c => c.Meanings).ThenInclude(m => m.Language)
                .Include(vt => vt.Verse)
                    .ThenInclude(v => v.Transliterations).ThenInclude(t => t.Language)
                
                .Include(vt => vt.Verse)
                    .ThenInclude(v => v.Translations).ThenInclude(t => t.Footnotes)
                .Include(vt => vt.Verse)
                    .ThenInclude(v => v.Translations).ThenInclude(t => t.Language)
                .Include(vt => vt.Verse)
                    .ThenInclude(v => v.Translations).ThenInclude(t => t.Translation).ThenInclude(t => t.Language)
                .Include(vt => vt.Verse)
                    .ThenInclude(v => v.Translations).ThenInclude(t => t.Translation).ThenInclude(t => t.Authors).ThenInclude(a => a.Language)
                .Include(vt => vt.Verse)
                    .ThenInclude(v => v.Translations).ThenInclude(t => t.Translation).ThenInclude(t => t.Authors).ThenInclude(a => a.NameTranslations).ThenInclude(nt => nt.Language)
                
                .ToListAsync();

            var orderedTranslations = orderedTranslationIds
                .Select(id => translationsUnordered.FirstOrDefault(vt => vt.Id == id))
                .Where(vt => vt != null)
                .Select(vt => vt!.ToWithVerse())
                .ToList();

            var result = new SearchResult(orderedTranslations);

            await cacheService.Save(cacheKey, result, TimeSpan.FromDays(30));

            return Ok(new { data = result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in text search. Query: {Query}", query);
            return StatusCode(500, "Internal server error.");
        }
    }
}