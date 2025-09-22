using DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Services;
using ScriptiumBackend.Interface;
using ScriptiumBackend.DB;

namespace ScriptiumBackend.Controllers.Book;

[ApiController, Route("book"), EnableRateLimiting(policyName: "StaticControllerRateLimit")]
public class BookController(ApplicationDbContext db, ICacheService cacheService, ILogger<BookController> logger)
    : ControllerBase
{
    private readonly ApplicationDbContext _db = db ?? throw new ArgumentNullException(nameof(db));

    private readonly ICacheService _cacheService =
        cacheService ?? throw new ArgumentNullException(nameof(cacheService));

    private readonly ILogger<BookController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));


    [HttpGet("")]
    public async Task<IActionResult> GetAllBooks()
    {
        string requestPath = Request.Path.ToString();

        try
        {
            if (await _cacheService.GetCachedDataAsync<List<BookCoverDto>>(requestPath) is { } cachedData)
            {
                _logger.LogInformation("Cache hit for URL: {RequestPath}", requestPath);
                return Ok(new { data = cachedData });
            }

            List<BookCoverDto> books = await _db.Books
                .AsNoTracking()
                .Include(b => b.Meanings).ThenInclude(m => m.Language)
                .Select(b => b.ToBookCoverDto())
                .ToListAsync();

            if (books.Count == 0)
            {
                _logger.LogWarning("No books found in database.");
                return NotFound(new { message = "No books found." });
            }

            await _cacheService.SetCacheDataAsync(requestPath, books);
            _logger.LogInformation("Data cached for URL: {RequestPath}", requestPath);

            return Ok(new { data = books });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing GetAllBooks.");
            return StatusCode(500, new { message = "An internal server error occurred." });
        }
    }


    [HttpGet("{bookMeaning}")]
    public async Task<IActionResult> GetBook([FromRoute] string bookMeaning)
    {
        try
        {
            string requestPath = Request.Path.ToString();


            BookCoverOneLevelLowerDto? cache =
                await _cacheService.GetCachedDataAsync<BookCoverOneLevelLowerDto>(requestPath);


            if (cache != null)
            {
                _logger.LogInformation($"Cache data with URL {requestPath} is found. Sending.");
                return Ok(new { data = cache });
            }

            BookCoverOneLevelLowerDto? data = await _db.BookMeanings.AsNoTracking()
                .Where(bm => bm.Meaning == bookMeaning)
                .Include(bm => bm.Book).ThenInclude(b => b.Meanings).ThenInclude(m => m.Language)
                .Include(bm => bm.Book.Nodes).ThenInclude(bm => bm.Meanings).ThenInclude(bn => bn.Language)
                .Select(bm => bm.Book).Select(b => b.ToBookCoverOneLevelLowerDto()).FirstOrDefaultAsync();


            if (data is null) return NotFound(new { message = "Book not found." });

            await _cacheService.SetCacheDataAsync(requestPath, data);
            _logger.LogInformation($"Cache data for URL {requestPath} is renewing");

            return Ok(new { data });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "Something went wrong.");
        }
    }

    [HttpGet("{bookMeaning}/{partMeaning}")]
    public async Task<IActionResult> GetSectionsOfBook(
        [FromRoute] string bookMeaning,
        [FromRoute] string partMeaning)
    {
        string requestPath = Request.Path.ToString();

        if (await _cacheService.GetCachedDataAsync<BookNodeOneLevelUpperBookAndOneLevelLowerCoverDto>(requestPath) is
            { } coverDto)
        {
            return Ok(new { data = coverDto });
        }

        if (await _cacheService.GetCachedDataAsync<BookNodeOneLevelUpperBookAndOneLevelLowerTextCoverDto>(requestPath)
            is { } textDto)
        {
            return Ok(new { data = textDto });
        }


        var partNode = await _db.BookNodes
            .AsNoTracking()
            .Include(n => n.Meanings).ThenInclude(m => m.Language)
            .Include(n => n.Texts).ThenInclude(t => t.TranslationTexts).ThenInclude(ttx => ttx.Footnotes)
            .Include(n => n.ChildNodes).ThenInclude(c => c.Meanings).ThenInclude(m => m.Language)
            .Include(n => n.Book).ThenInclude(b => b!.Meanings).ThenInclude(m => m.Language)
            .Where(n =>
                n.Meanings.Any(m => m.Meaning == partMeaning) &&
                n.Book != null &&
                n.Book.Meanings.Any(parent =>
                    parent.Meaning == bookMeaning &&
                    parent.Language.LangCode == n.Meanings.First().Language.LangCode))
            .FirstOrDefaultAsync();

        if (partNode is null)
            return NotFound(new { message = "BookNode not found" });

        object? data = partNode.ChildNodes.Count > 0
            ? partNode.ToBookNodeOneLevelUpperBookAndOneLevelLowerCoverDto()
            : partNode.Texts.Count > 0
                ? partNode.ToBookNodeOneLevelUpperBookAndOneLevelLowerTextCoverDto()
                : null;

        if (data is null)
        {
            _logger.LogError("BookNode with no children and no text: {id}", partNode.Id);
            return StatusCode(500, new { message = "Invalid node: no children or text" });
        }

        // 4. Cache ve sonuç
        await _cacheService.SetCacheDataAsync(requestPath, data);
        _logger.LogInformation($"Cached: {requestPath}");

        return Ok(new { data });
    }

    [HttpGet("{bookMeaning}/{partMeaning}/{subPartMeaning}")]
    public async Task<IActionResult> GetChapterNodeWithHierarchy(
        [FromRoute] string bookMeaning,
        [FromRoute] string partMeaning,
        [FromRoute] string subPartMeaning)
    {
        string requestPath = Request.Path.ToString();

        var coverDto =
            await _cacheService.GetCachedDataAsync<BookNodeTwoLevelUpperBookAndOneLevelLowerDto>(requestPath);
        if (coverDto?.Nodes is { Count: > 0 })
        {
            _logger.LogInformation($"Cache hit (cover): {requestPath}");
            return Ok(new { data = coverDto });
        }

        var textDto =
            await _cacheService.GetCachedDataAsync<BookNodeTwoLevelUpperBookAndOneLevelLowerTextCoverDto>(requestPath);
        if (textDto?.Texts is { Count: > 0 })
        {
            _logger.LogInformation($"Cache hit (text): {requestPath}");
            return Ok(new { data = textDto });
        }


        var subPartNode = await _db.BookNodes
            .AsNoTracking()
            .Include(n => n.Texts)
            .Include(n => n.Meanings).ThenInclude(m => m.Language)
            .Include(n => n.Texts).ThenInclude(t => t.TranslationTexts).ThenInclude(ttx => ttx.Footnotes)
            .Include(n => n.ChildNodes).ThenInclude(c => c.Meanings).ThenInclude(m => m.Language)

            // 1st level parent
            .Include(n => n.ParentBookNode)
            .ThenInclude(p => p.Meanings).ThenInclude(m => m.Language)

            // book attached to grandparent
            .Include(n => n.ParentBookNode)
            .ThenInclude(gp => gp.Book)
            .ThenInclude(b => b.Meanings).ThenInclude(m => m.Language)
            .Where(n =>
                n.Meanings.Any(m => m.Meaning == subPartMeaning) &&
                n.ParentBookNode != null &&
                n.ParentBookNode.Meanings.Any(m => m.Meaning == partMeaning) &&
                n.ParentBookNode.Book != null &&
                n.ParentBookNode.Book.Meanings.Any(m => m.Meaning == bookMeaning))
            .FirstOrDefaultAsync();


        if (subPartNode is null)
            return NotFound(new { message = "BookNode not found" });

        object? data = subPartNode.Texts.Count > 0
            ? subPartNode.ToBookNodeTwoLevelUpperBookAndOneLevelLowerTextCoverDto()
            : subPartNode.ChildNodes.Count > 0
                ? subPartNode.ToBookNodeTwoLevelUpperBookAndOneLevelLowerDto()
                : null;

        if (data is null)
        {
            _logger.LogError("BookNode with no children and no text: {id}", subPartNode.Id);
            return StatusCode(500, new { message = "Invalid node: no children or text" });
        }

        await _cacheService.SetCacheDataAsync(requestPath, data);
        _logger.LogInformation($"Cached: {requestPath}");

        return Ok(new { data });
    }

    [HttpGet("{bookMeaning}/{partMeaning}/{subPartMeaning}/{divisionMeaning}")]
    public async Task<IActionResult> GetChapterNodeWithHierarchy(
        [FromRoute] string bookMeaning,
        [FromRoute] string partMeaning,
        [FromRoute] string subPartMeaning,
        [FromRoute] string divisionMeaning
    )
    {
        string requestPath = Request.Path.ToString();

        var coverDto =
            await _cacheService.GetCachedDataAsync<BookNodeThreeLevelUpperBookAndOneLevelLowerCoverDto>(requestPath);
        if (coverDto?.Nodes is { Count: > 0 })
        {
            _logger.LogInformation($"Cache hit (cover): {requestPath}");
            return Ok(new { data = coverDto });
        }

        var textDto =
            await _cacheService
                .GetCachedDataAsync<BookNodeThreeLevelUpperBookAndOneLevelLowerTextCoverDto>(requestPath);
        if (textDto?.Texts is { Count: > 0 })
        {
            _logger.LogInformation($"Cache hit (text): {requestPath}");
            return Ok(new { data = textDto });
        }


        var sectionNode = await _db.BookNodes
            .AsNoTracking()
            .Include(n => n.Texts).ThenInclude(t => t.TranslationTexts).ThenInclude(tts => tts.Translation)
            .ThenInclude(t => t.TranslationBookTranslators).ThenInclude(tbt => tbt.Translator)
            .ThenInclude(t => t.Language)
            .Include(n => n.Texts).ThenInclude(t => t.TranslationTexts).ThenInclude(tts => tts.Translation)
            .ThenInclude(t => t.Language)
            .Include(n => n.Texts).ThenInclude(t => t.TranslationTexts).ThenInclude(ttx => ttx.Footnotes)
            .Include(n => n.Meanings).ThenInclude(m => m.Language)
            .Include(n => n.ChildNodes).ThenInclude(c => c.Meanings).ThenInclude(m => m.Language)

            // 1st level parent
            .Include(n => n.ParentBookNode)
            .ThenInclude(p => p.Meanings).ThenInclude(m => m.Language)
            // 2nd level parent
            .Include(n => n.ParentBookNode)
            .ThenInclude(gp => gp.ParentBookNode)
            .ThenInclude(p => p.Meanings).ThenInclude(m => m.Language)

            // book attached to grandparent
            .Include(n => n.ParentBookNode)
            .ThenInclude(p => p.ParentBookNode)
            .ThenInclude(gp => gp.Book)
            .ThenInclude(b => b.Meanings).ThenInclude(m => m.Language)
            .Where(n =>
                (n.Meanings.Any(m => m.Meaning == divisionMeaning) || n.Name == divisionMeaning) &&
                n.ParentBookNode != null &&
                n.ParentBookNode.Meanings.Any(m => m.Meaning == subPartMeaning) &&
                n.ParentBookNode.ParentBookNode != null &&
                n.ParentBookNode.ParentBookNode.Meanings.Any(m => m.Meaning == partMeaning) &&
                n.ParentBookNode.ParentBookNode.Book != null &&
                n.ParentBookNode.ParentBookNode.Book.Meanings.Any(m => m.Meaning == bookMeaning))
            .FirstOrDefaultAsync();


        if (sectionNode is null)
            return NotFound(new { message = "BookNode not found" });

        object? data = sectionNode.Texts.Count > 0
            ? sectionNode.ToBookNodeThreeLevelUpperBookAndOneLevelLowerTextCoverDto()
            : sectionNode.ChildNodes.Count > 0
                ? sectionNode.ToBookNodeThreeLevelUpperBookAndOneLevelLowerDto()
                : null;

        if (data is null)
        {
            _logger.LogError("BookNode with no children and no text: {id}", sectionNode.Id);
            return StatusCode(500, new { message = "Invalid node: no children or text" });
        }

        await _cacheService.SetCacheDataAsync(requestPath, data);
        _logger.LogInformation($"Cached: {requestPath}");

        return Ok(new { data });
    }

    [HttpGet("{bookMeaning}/{partMeaning}/{subPartMeaning}/{divisionMeaning}/{sectionMeaning}")]
    public async Task<IActionResult> GetChapterNodeWithHierarchy(
        [FromRoute] string bookMeaning,
        [FromRoute] string partMeaning,
        [FromRoute] string subPartMeaning,
        [FromRoute] string divisionMeaning,
        [FromRoute] string sectionMeaning
    )
    {
        string requestPath = Request.Path.ToString();


        var coverDto =
            await _cacheService.GetCachedDataAsync<BookNodeFourLevelUpperBookAndOneLevelLowerCoverDto>(requestPath);
        if (coverDto?.Nodes is { Count: > 0 })
        {
            _logger.LogInformation($"Cache hit (cover): {requestPath}");
            return Ok(new { data = coverDto });
        }

        var textDto =
            await _cacheService.GetCachedDataAsync<BookNodeFourLevelUpperBookAndOneLevelLowerTextCoverDto>(requestPath);
        if (textDto?.Texts is { Count: > 0 })
        {
            _logger.LogInformation($"Cache hit (text): {requestPath}");
            return Ok(new { data = textDto });
        }


        var sectionNode = await _db.BookNodes
            .AsNoTracking()
            .AsSplitQuery()
            .Include(n => n.Meanings).ThenInclude(m => m.Language)
            .Include(n => n.Texts).ThenInclude(t => t.TranslationTexts).ThenInclude(tts => tts.Translation)
            .ThenInclude(t => t.TranslationBookTranslators).ThenInclude(tbt => tbt.Translator)
            .ThenInclude(t => t.Language)
            .Include(n => n.Texts).ThenInclude(t => t.TranslationTexts).ThenInclude(tts => tts.Translation)
            .ThenInclude(t => t.Language)
            .Include(n => n.Texts).ThenInclude(t => t.TranslationTexts).ThenInclude(ttx => ttx.Footnotes)
            .Include(n => n.Meanings).ThenInclude(m => m.Language)
            .Include(n => n.ChildNodes).ThenInclude(c => c.Meanings).ThenInclude(m => m.Language)

            // 1st level parent
            .Include(n => n.ParentBookNode)
            .ThenInclude(p => p.Meanings).ThenInclude(m => m.Language)
            // 2nd level parent
            .Include(n => n.ParentBookNode)
            .ThenInclude(gp => gp.ParentBookNode)
            .ThenInclude(gp => gp.Meanings).ThenInclude(m => m.Language)

            // 3nd level parent and book attached to it.
            .Include(n => n.ParentBookNode)
            .ThenInclude(p => p.ParentBookNode)
            .ThenInclude(gp => gp.ParentBookNode)
            .ThenInclude(gpp => gpp.Book)
            .ThenInclude(b => b.Meanings).ThenInclude(m => m.Language)

            //Meaning of 3.nd level
            .Include(n => n.ParentBookNode)
            .ThenInclude(p => p.ParentBookNode)
            .ThenInclude(gp => gp.ParentBookNode)
            .ThenInclude(gpp => gpp.Meanings).ThenInclude(m => m.Language) // ✅ 
            .Where(n =>
                (n.Meanings.Any(m => m.Meaning == sectionMeaning) || n.Name == sectionMeaning) &&
                (n.ParentBookNode.Meanings.Any(m => m.Meaning == divisionMeaning) ||
                 n.ParentBookNode.Name == divisionMeaning) &&
                n.ParentBookNode.ParentBookNode != null &&
                n.ParentBookNode.ParentBookNode.Meanings.Any(m => m.Meaning == subPartMeaning) &&
                n.ParentBookNode.ParentBookNode.ParentBookNode != null &&
                n.ParentBookNode.ParentBookNode.ParentBookNode.Meanings.Any(m => m.Meaning == partMeaning) &&
                n.ParentBookNode.ParentBookNode.ParentBookNode.Book != null &&
                n.ParentBookNode.ParentBookNode.ParentBookNode.Book.Meanings.Any(m => m.Meaning == bookMeaning))
            .FirstOrDefaultAsync();


        if (sectionNode is null)
            return NotFound(new { message = "BookNode not found" });

        object? data = sectionNode.Texts.Count > 0
            ? sectionNode.ToBookNodeFourLevelUpperBookAndOneLevelLowerTextCoverDto()
            : sectionNode.ChildNodes.Count > 0
                ? sectionNode.ToBookNodeFourLevelUpperBookAndOneLevelLowerCoverDto()
                : null;

        if (data is null)
        {
            _logger.LogError("BookNode with no children and no text: {id}", sectionNode.Id);
            return StatusCode(500, new { message = "Invalid node: no children or text" });
        }

        await _cacheService.SetCacheDataAsync(requestPath, data);
        _logger.LogInformation($"Cached: {requestPath}");

        return Ok(new { data });
    }


    [HttpGet("{bookMeaning}/{partMeaning}/{subPartMeaning}/{divisionMeaning}/{sectionMeaning}/{chapterMeaning}")]
    public async Task<IActionResult> GetChapterNodeWithHierarchy(
        [FromRoute] string bookMeaning,
        [FromRoute] string partMeaning,
        [FromRoute] string subPartMeaning,
        [FromRoute] string divisionMeaning,
        [FromRoute] string sectionMeaning,
        [FromRoute] string chapterMeaning
    )
    {
        string requestPath = Request.Path.ToString();


        var coverDto =
            await _cacheService.GetCachedDataAsync<BookNodeFiveLevelUpperBookAndOneLevelLowerCoverDto>(requestPath);
        if (coverDto?.Nodes is { Count: > 0 })
        {
            _logger.LogInformation("Cache hit (cover): {requestPath}", requestPath);
            return Ok(new { data = coverDto });
        }

        var textDto =
            await _cacheService.GetCachedDataAsync<BookNodeFiveLevelUpperBookAndOneLevelLowerTextCoverDto>(requestPath);
        if (textDto?.Texts is { Count: > 0 })
        {
            _logger.LogInformation("Cache hit (text): {requestPath}", requestPath);
            return Ok(new { data = textDto });
        }

        var chapterQuery =
            _db.BookNodes
                .AsNoTracking()
                .IgnoreQueryFilters()
                .AsSplitQuery()
                .Include(n => n.Meanings).ThenInclude(m => m.Language)
                .Include(n => n.Texts).ThenInclude(t => t.TranslationTexts).ThenInclude(tts => tts.Translation)
                .ThenInclude(t => t.TranslationBookTranslators).ThenInclude(tbt => tbt.Translator)
                .ThenInclude(t => t.Language)
                .Include(n => n.Texts).ThenInclude(t => t.TranslationTexts).ThenInclude(tts => tts.Translation)
                .ThenInclude(t => t.Language)
                .Include(n => n.Texts).ThenInclude(t => t.TranslationTexts).ThenInclude(ttx => ttx.Footnotes)
                .Include(n => n.Meanings).ThenInclude(m => m.Language)
                .Include(n => n.ChildNodes).ThenInclude(c => c.Meanings).ThenInclude(m => m.Language)
                // 1st level parent
                .Include(n => n.ParentBookNode).ThenInclude(p => p.Meanings).ThenInclude(m => m.Language)
                // 2nd level parent
                .Include(n => n.ParentBookNode).ThenInclude(gp => gp.ParentBookNode).ThenInclude(gp => gp.Meanings)
                .ThenInclude(m => m.Language)
                // 3rd
                .Include(n => n.ParentBookNode).ThenInclude(p => p.ParentBookNode).ThenInclude(gp => gp.ParentBookNode)
                .ThenInclude(gpp => gpp.Meanings).ThenInclude(m => m.Language)
                // 4th + book
                .Include(n => n.ParentBookNode).ThenInclude(p => p.ParentBookNode).ThenInclude(gp => gp.ParentBookNode)
                .ThenInclude(ggp => ggp.ParentBookNode).ThenInclude(ggp => ggp.Book).ThenInclude(b => b.Meanings)
                .ThenInclude(m => m.Language)
                // 4th level meanings (explicit)
                .Include(n => n.ParentBookNode).ThenInclude(p => p.ParentBookNode).ThenInclude(gp => gp.ParentBookNode)
                .ThenInclude(gpp => gpp.ParentBookNode).ThenInclude(gpp => gpp.Meanings).ThenInclude(m => m.Language)
                .Where(n =>
                    // 0: Chapter
                    (n.Meanings.Any(m => m.Meaning == chapterMeaning) || n.Name == chapterMeaning)
                    // 1: Section
                    && n.ParentBookNode != null
                    && (n.ParentBookNode.Meanings.Any(m => m.Meaning == sectionMeaning) ||
                        n.ParentBookNode.Name == sectionMeaning)
                    // 2: Division
                    && n.ParentBookNode.ParentBookNode != null
                    && (n.ParentBookNode.ParentBookNode.Meanings.Any(m => m.Meaning == divisionMeaning) ||
                        n.ParentBookNode.ParentBookNode.Name == divisionMeaning)
                    // 3: SubPart
                    && n.ParentBookNode.ParentBookNode.ParentBookNode != null
                    && (n.ParentBookNode.ParentBookNode.ParentBookNode.Meanings.Any(m => m.Meaning == subPartMeaning) ||
                        n.ParentBookNode.ParentBookNode.ParentBookNode.Name == subPartMeaning)
                    // 4: Part
                    && n.ParentBookNode.ParentBookNode.ParentBookNode.ParentBookNode != null
                    && (n.ParentBookNode.ParentBookNode.ParentBookNode.ParentBookNode.Meanings.Any(m =>
                            m.Meaning == partMeaning) ||
                        n.ParentBookNode.ParentBookNode.ParentBookNode.ParentBookNode.Name == partMeaning)
                    // Book
                    && n.ParentBookNode.ParentBookNode.ParentBookNode.ParentBookNode.Book != null
                    && n.ParentBookNode.ParentBookNode.ParentBookNode.ParentBookNode.Book.Meanings.Any(m =>
                        m.Meaning == bookMeaning)
                );


        var chapterNode = await chapterQuery.FirstOrDefaultAsync();
        if (chapterNode is null)
            return NotFound(new { message = "BookNode not found" });

        object? data = chapterNode.Texts.Count > 0
            ? chapterNode.ToBookNodeFiveLevelUpperBookAndOneLevelLowerTextCoverDto()
            : chapterNode.ChildNodes.Count > 0
                ? chapterNode.ToBookNodeFiveLevelUpperBookAndOneLevelLowerCoverDto()
                : null;

        if (data is null)
        {
            _logger.LogError("BookNode with no children and no text: {id}", chapterNode.Id);
            return StatusCode(500, new { message = "Invalid node: no children or text" });
        }

        await _cacheService.SetCacheDataAsync(requestPath, data);
        _logger.LogInformation("Cached: {requestPath}", requestPath);

        return Ok(new { data });
    }
}