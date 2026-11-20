using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Db;
using ScriptiumBackend.Dto.Derived.Islam.Hadith.ArabicOnly;
using ScriptiumBackend.Services.ServiceInterfaces;

namespace ScriptiumBackend.Controllers.Islam.Hadith.ArabicOnly;

/// <summary>A hadith "meaning": all narrations of one report across the books.</summary>
[ApiController, Route(HadithQueries.Route)]
public class HadithMeaningGroupController(
    ScriptiumDbContext db,
    ILogger<HadithMeaningGroupController> logger,
    ICacheService cacheService) : HadithControllerBase(cacheService, logger)
{
    private sealed class CompanionRow
    {
        public int NarratorId { get; init; }
        public int Count { get; init; }
    }

    [HttpGet("group/{id:int}")]
    public Task<IActionResult> Get([FromRoute, Range(1, int.MaxValue)] int id) => Cached<MeaningGroupComplete>(async () =>
    {
        var group = await db.HAoMeaningGroups.AsNoTracking().Where(g => g.Id == id).ToPlain().FirstOrDefaultAsync();
        if (group is null) return null;

        var hadiths = db.HAoHadiths.InGroup(id);

        // The last link of every chain is the companion (or earliest narrator).
        var companions = await db.Database.SqlQuery<CompanionRow>($"""
            SELECT top.i_h_ao_narrator_id AS "NarratorId", count(*)::int AS "Count"
            FROM i_h_ao_sanad sa
            JOIN LATERAL (
                SELECT x.i_h_ao_narrator_id FROM i_h_ao_sanad_narrator x
                WHERE x.i_h_ao_sanad_id = sa.id ORDER BY x.position DESC LIMIT 1
            ) top ON true
            WHERE sa.i_h_ao_meaning_group_id = {id}
            GROUP BY 1 ORDER BY 2 DESC LIMIT 20
            """).ToListAsync();
        var companionIds = companions.Select(c => c.NarratorId).ToList();
        var companionNarrators = await db.HAoNarrators.AsNoTracking().Where(n => companionIds.Contains(n.Id))
            .ToPlain().ToDictionaryAsync(n => n.Id);

        return new MeaningGroupComplete
        {
            Group = group,
            TopicPaths = await db.TopicPathsOfGroup(id, limit: 10),
            GradeDistribution = await hadiths.GradeDistribution(),
            Books = await db.BookDistribution(hadiths, 30),
            ChainCount = await db.HAoSanads.InGroup(id).CountAsync(),
            Companions = companions.Where(c => companionNarrators.ContainsKey(c.NarratorId))
                .Select(c => new NarratorLinkItem { Narrator = companionNarrators[c.NarratorId], ChainCount = c.Count })
                .ToList(),
        };
    });

    [HttpGet("group/{id:int}/hadiths")]
    public Task<IActionResult> Hadiths(
        [FromRoute, Range(1, int.MaxValue)] int id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] int? book = null,
        [FromQuery, EnumDataType(typeof(HadithGrade))] HadithGrade? grade = null) => Uncached(async () =>
    {
        if (!await db.HAoMeaningGroups.AnyAsync(g => g.Id == id)) return null;

        var q = db.HAoHadiths.AsNoTracking().InGroup(id);
        if (book is not null) q = q.InBook(book.Value);
        if (grade is not null) q = q.Where(h => h.Grade == grade);

        return await q.InBookOrder().ToListItems().ToPagedAsync(page, pageSize);
    });

    /// <summary>All chains of the meaning merged into one isnad tree.</summary>
    [HttpGet("group/{id:int}/isnad")]
    public Task<IActionResult> Isnad([FromRoute, Range(1, int.MaxValue)] int id) => Cached<IsnadGraph>(async () =>
        await db.HAoMeaningGroups.AnyAsync(g => g.Id == id) ? await IsnadGraphBuilder.ForGroup(db, id) : null);
}
