using ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Controllers.Islam.Hadith.ArabicOnly;
using ScriptiumBackend.Db;
using ScriptiumBackend.Dto.Derived.Islam.Hadith.ArabicOnly;
using ScriptiumBackend.Services.ServiceInterfaces;
using ScriptiumBackend.Utils.Text;
using HadithModel = ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Hadith;

namespace ScriptiumBackend.Controllers.Islam.JarhTadil.ArabicOnly;

/// <summary>Narrators (rijal): biographies, critics' verdicts, teachers, students and hadiths.</summary>
[ApiController, Route(JarhTadilRoutes.ArabicOnly)]
public class NarratorController(
    ScriptiumDbContext db,
    ILogger<NarratorController> logger,
    ICacheService cacheService) : HadithControllerBase(cacheService, logger)
{
    private const int LinkPreview = 24;

    /// <summary>Browse or search narrators, most prolific first.</summary>
    [HttpGet("narrators")]
    public Task<IActionResult> List(
        [FromQuery, StringLength(100)] string? query = null,
        [FromQuery, EnumDataType(typeof(NarratorRank))] NarratorRank? rank = null,
        [FromQuery, Range(0, 100)] short? generation = null,
        [FromQuery] bool? bukhari = null,
        [FromQuery] bool? muslim = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 30) => Uncached(async () =>
    {
        var q = db.HAoNarrators.AsNoTracking().Where(n => !n.IsStub);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var normalized = ArabicNormalizer.Normalize(query);
            if (normalized.Length > 0)
                // Normalised text only contains letters, digits and spaces: no LIKE escaping needed.
                q = q.Where(n => EF.Functions.Like(n.SearchName, "%" + normalized + "%"));
        }

        if (rank is not null) q = q.Where(n => n.RankNo == rank);
        if (generation is not null) q = q.Where(n => n.Generation == generation);
        if (bukhari is true) q = q.Where(n => n.IsBukhari);
        if (muslim is true) q = q.Where(n => n.IsMuslim);

        return await q.OrderByDescending(n => n.HadithCount).ThenBy(n => n.Id).ToPlain()
            .ToPagedAsync(page, pageSize);
    });

    [HttpGet("narrator/{id:int}")]
    public Task<IActionResult> Get([FromRoute, Range(1, int.MaxValue)] int id) => Cached<NarratorComplete>(async () =>
    {
        var n = await db.HAoNarrators.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (n is null) return null;

        var statements = await db.HAoScholarStatements.AsNoTracking()
            .AboutNarrator(id)
            .OrderBy(s => s.Scholar.DeathYear).ThenBy(s => s.Id)
            .Select(s => new StatementWithScholar
            {
                Id = s.Id,
                Text = s.Text,
                Scholar = new ScholarPlain
                {
                    Id = s.Scholar.Id,
                    KnownAs = s.Scholar.KnownAs,
                    Name = s.Scholar.Name,
                    DeathYear = s.Scholar.DeathYear,
                    RankNo = s.Scholar.RankNo,
                    StatementCount = s.Scholar.StatementCount,
                },
            })
            .ToListAsync();

        var hadiths = HadithsOf(id);

        return new NarratorComplete
        {
            Id = n.Id,
            Name = n.Name,
            Nickname = n.Nickname,
            RankNo = n.RankNo,
            Rank = n.Rank,
            Generation = n.Generation,
            IsBukhari = n.IsBukhari,
            IsMuslim = n.IsMuslim,
            HasIkhtilat = n.HasIkhtilat,
            HasTadlis = n.HasTadlis,
            RiwayaCount = n.RiwayaCount,
            BirthYear = n.BirthYear,
            BirthYearRaw = n.BirthYearRaw,
            DeathYear = n.DeathYear,
            DeathYearRaw = n.DeathYearRaw,
            AgeRaw = n.AgeRaw,
            Profession = n.Profession,
            Lineage = n.Lineage,
            Residence = n.Residence,
            DeathPlace = n.DeathPlace,
            IsStub = n.IsStub,
            HadithCount = n.HadithCount,
            Statements = statements,
            Teachers = await db.Teachers(id).Take(LinkPreview).ToListAsync(),
            TeacherCount = await db.HAoNarratorLinks.TeachersOf(id).CountAsync(),
            Students = await db.Students(id).Take(LinkPreview).ToListAsync(),
            StudentCount = await db.HAoNarratorLinks.StudentsOf(id).CountAsync(),
            Books = await db.BookDistribution(hadiths, 20),
            GradeDistribution = await hadiths.GradeDistribution(),
        };
    });

    /// <summary>Hadiths whose chains contain the narrator, in book order.</summary>
    [HttpGet("narrator/{id:int}/hadiths")]
    public Task<IActionResult> Hadiths(
        [FromRoute, Range(1, int.MaxValue)] int id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] int? book = null,
        [FromQuery, EnumDataType(typeof(HadithGrade))] HadithGrade? grade = null) => Uncached(async () =>
    {
        if (!await db.HAoNarrators.AnyAsync(n => n.Id == id)) return null;

        var q = HadithsOf(id).AsNoTracking();
        if (book is not null) q = q.InBook(book.Value);
        if (grade is not null) q = q.Where(h => h.Grade == grade);

        return await q.InBookOrder().ToListItems().ToPagedAsync(page, pageSize);
    });

    [HttpGet("narrator/{id:int}/teachers")]
    public Task<IActionResult> Teachers([FromRoute] int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 50) =>
        Uncached(() => db.Teachers(id).ToPagedAsync(page, pageSize));

    [HttpGet("narrator/{id:int}/students")]
    public Task<IActionResult> Students([FromRoute] int id, [FromQuery] int page = 1, [FromQuery] int pageSize = 50) =>
        Uncached(() => db.Students(id).ToPagedAsync(page, pageSize));

    private IQueryable<HadithModel> HadithsOf(int narratorId)
    {
        var ids = db.HAoSanadNarrators.HadithIdsOfNarrator(narratorId);
        return db.HAoHadiths.Where(h => ids.Contains(h.Id));
    }
}
