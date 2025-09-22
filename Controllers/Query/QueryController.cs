using DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Controllers.Validation;
using ScriptiumBackend.Services;
using System.Linq;
using ScriptiumBackend.Interface;
using ScriptiumBackend.DB;
using ScriptiumBackend.Entities;

namespace ScriptiumBackend.Controllers.QueryHandler
{
    [ApiController, Route("query"), EnableRateLimiting(policyName: "StaticControllerRateLimiter")]
    public class QueryController(ISearchService searchService, ICacheService cacheService, ILogger<QueryController> logger) : ControllerBase
    {
        private readonly ISearchService _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
        private readonly ICacheService _cacheService = cacheService ??  throw new ArgumentNullException(nameof(cacheService));
        private readonly ILogger<QueryController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));



        [HttpGet("search")]
        public async Task<IActionResult> GetQueryResult([FromQuery] string query)
        {
            if (query.Length < 3 || query.Length > 126) return NotFound();

            string requestPath = Request.Path + Request.QueryString;

            SearchResultDto? cached = await _cacheService.GetCachedDataAsync<SearchResultDto>(requestPath);
           
            if (cached is not null)
            {
                _logger.LogInformation("Cache data with URL {Url} is found. Sending.", requestPath);
                return Ok(new { data = cached });
            }

            SearchResultDto result = await _searchService.SearchAsync(query);

            await _cacheService.SetCacheDataAsync(requestPath, result);
            _logger.LogInformation("Cache data for URL {Url} is renewing", requestPath);

            return Ok(new { data = result });
        }
    }
}
