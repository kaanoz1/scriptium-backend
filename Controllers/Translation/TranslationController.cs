using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using scriptium_backend_dotnet.DB;
using scriptium_backend_dotnet.Services;

namespace scriptium_backend_dotnet.Controllers.TranslatorsHandler
{

    [ApiController, Route("translations"), EnableRateLimiting(policyName: "StaticControllerRateLimiter")]
    public class TranslationController(ApplicationDBContext db, ICacheService cacheService, ILogger<TranslationController> logger) : ControllerBase
    {
        private readonly ApplicationDBContext _db = db ?? throw new ArgumentNullException(nameof(db));
        private readonly ICacheService _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        private readonly ILogger<TranslationController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        [HttpGet("")]
        public async Task<IActionResult> GetTranslators()
        {
            string requestPath = Request.Path.ToString();

            List<TranslationWithScriptureDTODTO>? cache = await _cacheService.GetCachedDataAsync<List<TranslationWithScriptureDTODTO>>(requestPath);

            if (cache != null)
            {
                _logger.LogInformation($"Cache data with URL {requestPath} is found. Sending.");
                return Ok(new { data = cache });
            }

            List<TranslationWithScriptureDTODTO> data = await _db.Translation
            .AsNoTracking()
            .Include(t => t.Language)
            .Include(t => t.TranslatorTranslations).ThenInclude(tt => tt.Translator).ThenInclude(t => t.Language)
            .Include(t => t.Scripture).ThenInclude(s => s.Meanings).ThenInclude(m => m.Language)
            .AsSplitQuery()
            .Select(t => t.ToTranslationWithScriptureDTODTO()).ToListAsync();

            await _cacheService.SetCacheDataAsync(requestPath, data);

            _logger.LogInformation($"Cache data for URL {requestPath} is renewing");

            return Ok(new { data });

        }

        [HttpGet("{scriptureNumber}")]
        public async Task<IActionResult> GetTranslationOfScripture(short scriptureNumber)
        {
            string requestPath = Request.Path.ToString();


            List<TranslationDTO>? cache = await _cacheService.GetCachedDataAsync<List<TranslationDTO>>(requestPath);
            
            if (cache != null) //Checking cache
            {
                _logger.LogInformation($"Cache data with URL {requestPath} is found. Sending.");
                return Ok(new { data = cache });
            }

            List<TranslationDTO> data = await _db.Translation.Where(t => t.Scripture.Number == scriptureNumber)
                .Select(t => t.ToTranslationDTO()).ToListAsync();
            
            await _cacheService.SetCacheDataAsync(requestPath, data);
            
            _logger.LogInformation($"Cache data for URL {requestPath} is renewing");

            return Ok(new { data });
        }
    }
}