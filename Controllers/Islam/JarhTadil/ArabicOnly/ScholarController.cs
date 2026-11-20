using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Controllers.Islam.Hadith.ArabicOnly;
using ScriptiumBackend.Db;
using ScriptiumBackend.Dto.Derived.Islam.Hadith.ArabicOnly;
using ScriptiumBackend.Services.ServiceInterfaces;

namespace ScriptiumBackend.Controllers.Islam.JarhTadil.ArabicOnly;

/// <summary>Critics of narrators (jarh wa ta'dil) and their statements.</summary>
[ApiController, Route(JarhTadilRoutes.ArabicOnly)]
public class ScholarController(
    ScriptiumDbContext db,
    ILogger<ScholarController> logger,
    ICacheService cacheService) : HadithControllerBase(cacheService, logger)
{
    [HttpGet("scholars")]
    public Task<IActionResult> List() => Cached<ScholarList>(async () => new ScholarList
    {
        Scholars = await db.HAoScholars.AsNoTracking()
            .OrderByDescending(s => s.StatementCount).ThenBy(s => s.Id)
            .ToPlain().ToListAsync(),
    });

    [HttpGet("scholar/{id:int}")]
    public Task<IActionResult> Get([FromRoute, Range(1, int.MaxValue)] int id) => Cached<ScholarComplete>(async () =>
        await db.HAoScholars.AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new ScholarComplete
            {
                Id = s.Id,
                Name = s.Name,
                Nickname = s.Nickname,
                KnownAs = s.KnownAs,
                Title = s.Title,
                Generation = s.Generation,
                DeathYear = s.DeathYear,
                RankNo = s.RankNo,
                Rank = s.Rank,
                StatementCount = s.StatementCount,
                Notes = s.Notes,
            })
            .FirstOrDefaultAsync());

    /// <summary>What the scholar said about narrators, most prolific narrators first.</summary>
    [HttpGet("scholar/{id:int}/statements")]
    public Task<IActionResult> Statements(
        [FromRoute, Range(1, int.MaxValue)] int id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50) => Uncached(async () =>
    {
        if (!await db.HAoScholars.AnyAsync(s => s.Id == id)) return null;

        return await db.HAoScholarStatements.AsNoTracking()
            .ByScholar(id)
            .OrderByDescending(s => s.Narrator.HadithCount).ThenBy(s => s.Id)
            .Select(s => new StatementWithNarrator
            {
                Id = s.Id,
                Text = s.Text,
                Narrator = new NarratorPlain
                {
                    Id = s.Narrator.Id,
                    Name = s.Narrator.Name,
                    Nickname = s.Narrator.Nickname,
                    RankNo = s.Narrator.RankNo,
                    Rank = s.Narrator.Rank,
                    Generation = s.Narrator.Generation,
                    DeathYear = s.Narrator.DeathYear,
                    IsStub = s.Narrator.IsStub,
                    HadithCount = s.Narrator.HadithCount,
                },
            })
            .ToPagedAsync(page, pageSize);
    });
}
