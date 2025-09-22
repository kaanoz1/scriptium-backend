using ScriptiumBackend.Models;
using ScriptiumBackend.Entities;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System.IO;
using DTO;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Interface;
using ScriptiumBackend.DB;

namespace ScriptiumBackend.Services;

public class LuceneSearcherService : ISearchService
{
    private readonly string _indexPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LuceneIndex");
    private readonly LuceneVersion _luceneVersion = LuceneVersion.LUCENE_48;
    private readonly StandardAnalyzer _analyzer;
    private readonly FSDirectory _directory;
    private readonly ApplicationDbContext _db;

    public LuceneSearcherService(ApplicationDbContext db)
    {
        _analyzer = new StandardAnalyzer(_luceneVersion);
        _directory = FSDirectory.Open(new DirectoryInfo(_indexPath));


        ArgumentNullException.ThrowIfNull(db);
        _db = db;
    }

    public async Task<SearchResultDto> SearchAsync(string queryText)
    {
        using var reader = DirectoryReader.Open(_directory);
        var searcher = new IndexSearcher(reader);

        var parser = new MultiFieldQueryParser(_luceneVersion, ["text", "name"], _analyzer);
        var query = parser.Parse(QueryParser.Escape(queryText));

        var hits = searcher.Search(query, 20).ScoreDocs;

        var result = new SearchResultDto();


        var translationIds = new HashSet<long>();
        var sectionIds = new HashSet<long>();

        foreach (var hit in hits)
        {
            var doc = searcher.Doc(hit.Doc);
            if (!long.TryParse(doc.Get("id"), out var id)) continue;

            switch (doc.Get("type"))
            {
                case "translation": translationIds.Add(id); break;
                case "section": sectionIds.Add(id); break;
            }
        }


        if (translationIds.Any())
        {
            result.TranslationTexts.AddRange(await _db.TranslationTexts.Where(ttx => translationIds.Contains(ttx.Id))
                .Include(tt => tt.Translation).ThenInclude(t => t.Language)
                .Include(tt => tt.Translation).ThenInclude(t => t.TranslatorTranslations)
                .ThenInclude(t => t.Translator).ThenInclude(t => t.Language)
                .Include(tt => tt.FootNotes).ThenInclude(fn => fn.FootNoteText)
                .Include(tt => tt.Verse).ThenInclude(v => v.Chapter).ThenInclude(c => c.Section)
                .ThenInclude(s => s.Scripture).ThenInclude(s => s.Meanings).ThenInclude(m => m.Language)
                .Include(tt => tt.Verse).ThenInclude(v => v.Chapter).ThenInclude(c => c.Section)
                .ThenInclude(s => s.Meanings).ThenInclude(m => m.Language)
                .Include(tt => tt.Verse).ThenInclude(v => v.Chapter)
                .AsSplitQuery()
                .AsNoTracking()
                .Select(ttx => ttx.ToTranslationTextWithVerseUpperMeanDto()).ToListAsync());
        }

        if (sectionIds.Any())
        {
            result.Sections.AddRange(await _db.Sections.Where(s => sectionIds.Contains(s.Id))
                .Include(s => s.Scripture).ThenInclude(s => s.Meanings).ThenInclude(m => m.Language)
                .Include(s => s.Meanings).ThenInclude(m => m.Language)
                .AsNoTracking()
                .AsSplitQuery()
                .Select(s => s.ToSectionUpperMeanDto())
                .ToListAsync());
        }
        

        return result;
    }
}

