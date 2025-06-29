using DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using scriptium_backend_dotnet.Controllers.Validation;
using scriptium_backend_dotnet.DB;
using scriptium_backend_dotnet.Services;

namespace scriptium_backend_dotnet.Controllers.RootHandler
{

    [ApiController, Route("root"), EnableRateLimiting(policyName: "StaticControllerRateLimiter")]
    public class RootController(ApplicationDBContext db, ICacheService cacheService, ILogger<RootController> logger) : ControllerBase
    {
        private readonly ApplicationDBContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ICacheService _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        private readonly ILogger<RootController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));


        [HttpGet("{ScriptureNumber}/{RootLatin}")]
        public async Task<IActionResult> GetRoot([FromRoute] RootValidatedDTO model)
        {
            string requestPath = Request.Path.ToString();

            RootUpperDTO? cache = await _cacheService.GetCachedDataAsync<RootUpperDTO>(requestPath);
            
            if (cache != null)
            {
                _logger.LogInformation($"Cache data with URL {requestPath} is found. Sending.");
                return Ok(new { data = cache });
            }


            RootUpperDTO? data = await _db.Root
                .Where(r => r.Scripture.Number == model.ScriptureNumber && r.Latin == model.RootLatin)
                .AsNoTracking()
                .IgnoreAutoIncludes()
                .Include(r => r.Words)
                    .ThenInclude(w => w.Verse)
                        .ThenInclude(v => v.Transliterations).ThenInclude(t => t.Language)
                .Include(r => r.Words)
                    .ThenInclude(w => w.Verse)
                        .ThenInclude(v => v.TranslationTexts).ThenInclude(tt => tt.Translation).ThenInclude(t => t.Language)
                .Include(r => r.Words)
                    .ThenInclude(w => w.Verse)
                        .ThenInclude(v => v.TranslationTexts).ThenInclude(tt => tt.Translation).ThenInclude(t => t.TranslatorTranslations).ThenInclude(tt => tt.Translator).ThenInclude(t => t.Language)
                .Include(r => r.Words)
                    .ThenInclude(w => w.Verse)
                        .ThenInclude(v => v.Chapter)
                            .ThenInclude(c => c.Meanings)
                                .ThenInclude(c => c.Language) 
                .Include(r => r.Words)
                    .ThenInclude(w => w.Verse)
                        .ThenInclude(v => v.Chapter)
                            .ThenInclude(v => v.Section)
                                .ThenInclude(s => s.Meanings)
                                    .ThenInclude(m => m.Language)
                .Include(r => r.Words)
                    .ThenInclude(w => w.Verse)
                        .ThenInclude(v => v.Chapter)
                            .ThenInclude(c => c.Section)
                                .ThenInclude(s => s.Scripture)
                                    .ThenInclude(s => s.Meanings)
                                        .ThenInclude(m => m.Language)
                .AsSplitQuery()
                .Select(r => r.ToRootUpperDTO())
                .FirstOrDefaultAsync();

            if (data == null)
                return NotFound("There is no root matches with this information.");

            await _cacheService.SetCacheDataAsync(requestPath, data);
            _logger.LogInformation($"Cache data for URL {requestPath} is renewing");

            return Ok(new { data });
        }
    }
}