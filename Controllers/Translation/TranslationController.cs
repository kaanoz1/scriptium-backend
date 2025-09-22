using DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Services;
using ScriptiumBackend.Interface;
using ScriptiumBackend.DB;

namespace ScriptiumBackend.Controllers.TranslatorsHandler
{

    [ApiController, Route("translations"), EnableRateLimiting(policyName: "StaticControllerRateLimiter")]
    public class TranslationController(ApplicationDbContext db, ICacheService cacheService, ILogger<TranslationController> logger) : ControllerBase
    {
        private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ICacheService _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        private readonly ILogger<TranslationController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [HttpGet("")]
        public async Task<IActionResult> GetTranslators()
        {
            string requestPath = Request.Path.ToString();

            List<TranslationWithScriptureDtoDto>? cache = await _cacheService.GetCachedDataAsync<List<TranslationWithScriptureDtoDto>>(requestPath);

            if (cache != null)
            {
                _logger.LogInformation($"Cache data with URL {requestPath} is found. Sending.");
                return Ok(new { data = cache });
            }

            List<TranslationWithScriptureDtoDto> data = await _db.Translations
            .AsNoTracking()
            .Include(t => t.Language)
            .Include(t => t.TranslatorTranslations).ThenInclude(tt => tt.Translator).ThenInclude(t => t.Language)
            .Include(t => t.Scripture).ThenInclude(s => s.Meanings).ThenInclude(m => m.Language)
            .AsSplitQuery()
            .Select(t => t.ToTranslationWithScriptureDtoDto()).ToListAsync();

            await _cacheService.SetCacheDataAsync(requestPath, data);

            _logger.LogInformation($"Cache data for URL {requestPath} is renewing");

            return Ok(new { data });

        }

        [HttpGet("{scriptureNumber}")]
        public async Task<IActionResult> GetTranslationOfScripture(short scriptureNumber)
        {
            string requestPath = Request.Path.ToString();


            List<TranslationDto>? cache = await _cacheService.GetCachedDataAsync<List<TranslationDto>>(requestPath);
            
            if (cache != null) //Checking cache
            {
                _logger.LogInformation($"Cache data with URL {requestPath} is found. Sending.");
                return Ok(new { data = cache });
            }

            List<TranslationDto> data = await _db.Translations.Where(t => t.Scripture.Number == scriptureNumber)
                .Include(t => t.Language)
                .Include(t => t.TranslatorTranslations)
                .ThenInclude(tt => tt.Translator)
                .ThenInclude(t => t.Language)
                .Select(t => t.ToTranslationDto()).ToListAsync();
            
            await _cacheService.SetCacheDataAsync(requestPath, data);
            
            _logger.LogInformation($"Cache data for URL {requestPath} is renewing");

            return Ok(new { data });
        }
    }
}