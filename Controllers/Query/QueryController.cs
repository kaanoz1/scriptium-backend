using DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using scriptium_backend_dotnet.Controllers.Validation;
using scriptium_backend_dotnet.DB;
using scriptium_backend_dotnet.Services;
using System.Linq;

namespace scriptium_backend_dotnet.Controllers.QueryHandler
{
    [ApiController, Route("query"), EnableRateLimiting(policyName: "StaticControllerRateLimiter")]
    public class QueryController(ApplicationDBContext db, ICacheService cacheService, ILogger<QueryController> logger) : ControllerBase
    {
        private readonly ApplicationDBContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ICacheService _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        private readonly ILogger<QueryController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [HttpGet("search")]
        public async Task<IActionResult> GetQueryResult([FromQuery] string query)
        {

            if (query.Length > 126) return NotFound();

            const byte topCount = 10;

            List<TranslationTextWithVerseUpperMeanDTO> data;

            string requestPath = Request.Path.ToString() + Request.QueryString.ToString();

            List<TranslationTextWithVerseUpperMeanDTO>? cache = await _cacheService.GetCachedDataAsync<List<TranslationTextWithVerseUpperMeanDTO>>(requestPath);
            if (cache != null)
            {
                _logger.LogInformation($"Cache data with URL {requestPath} is found. Sending.");
                return Ok(new { data = cache });
            }

            string[] words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string? containsQuery = string.Join(" AND ", words.Select(w => $"\"{w}\""));


            data = await _db.TranslationText
                            .FromSqlRaw("SELECT * FROM translation_text WHERE CONTAINS(text, {0})", containsQuery)
                            .AsNoTracking()
                            .Take(topCount)
                            .OrderBy(tt => tt.Id)
                            .Include(tt => tt.Translation).ThenInclude(t => t.Language)
                            .Include(tt => tt.Translation).ThenInclude(t => t.TranslatorTranslations).ThenInclude(t => t.Translator).ThenInclude(t => t.Language)
                            .Include(tt => tt.FootNotes).ThenInclude(fn => fn.FootNoteText)
                            .Include(tt => tt.Verse).ThenInclude(v => v.Chapter).ThenInclude(c => c.Section).ThenInclude(s => s.Scripture).ThenInclude(s => s.Meanings).ThenInclude(m => m.Language)
                            .Include(tt => tt.Verse).ThenInclude(v => v.Chapter).ThenInclude(c => c.Section).ThenInclude(s => s.Meanings).ThenInclude(m => m.Language)
                            .Include(tt => tt.Verse).ThenInclude(v => v.Chapter)
                            .AsSplitQuery()
                            .Select(tt => tt.ToTranslationTextWithVerseUpperMeanDTO())
                            .ToListAsync();

            await _cacheService.SetCacheDataAsync(requestPath, data);
            _logger.LogInformation($"Cache data for URL {requestPath} is renewing");

            return Ok(new { data });
        }
    }
}
