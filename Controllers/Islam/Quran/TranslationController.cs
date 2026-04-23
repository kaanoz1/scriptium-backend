using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Db;
using ScriptiumBackend.Dto.Derived.Islam.Quranic.Translation;
using ScriptiumBackend.Services.ServiceInterfaces;

namespace ScriptiumBackend.Controllers.Islam.Quran;

[ApiController, Route("api/islam/quranic")]
public class TranslationController(
    ScriptiumDbContext db,
    ILogger<TranslationController> logger,
    ICacheService cacheService
)
    : Controller
{
    [HttpGet("translation/list")]
    public async Task<IActionResult> Search()
    {
        var cacheKey = Request.GetEncodedPathAndQuery();

        try
        {
            if (await cacheService.Get<List<Complete>>(cacheKey) is
                { } serializedCache)
            {
                logger.LogInformation("Cache has been found. Cache.Id: {Id}. Sending.", serializedCache.Raw.Id);
                return Ok(new { data = serializedCache.Data });
            }


            var qTranslations = await db.QTranslations
                .Include(t => t.Language)
                .Include(t => t.Authors).ThenInclude(a => a.Language)
                .Include(t => t.Authors).ThenInclude(a => a.NameTranslations).ThenInclude(nt => nt.Language)
                .Select(t => t.ToComplete()).ToListAsync();

            await cacheService.Save(cacheKey, qTranslations);

            return Ok(new
            {
                data = qTranslations
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in listing translation");
            return StatusCode(500, "Internal server error.");
        }
    }
}