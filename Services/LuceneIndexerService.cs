using ScriptiumBackend.Entities;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.EntityFrameworkCore;
using ScriptiumBackend.Models;
using ScriptiumBackend.DB;

namespace ScriptiumBackend.Services;

public class LuceneIndexerService
{
    private readonly string _indexPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "LuceneIndex");
    private readonly LuceneVersion _luceneVersion = LuceneVersion.LUCENE_48;
    private readonly StandardAnalyzer _analyzer;
    private readonly FSDirectory _directory;
    private readonly IServiceProvider _serviceProvider;

    public LuceneIndexerService(IServiceProvider serviceProvider)
    {
        _analyzer = new StandardAnalyzer(_luceneVersion);
        _directory = FSDirectory.Open(new DirectoryInfo(_indexPath));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public async Task RebuildAllIndicesAsync()
    {
        var config = new IndexWriterConfig(_luceneVersion, _analyzer)
        {
            OpenMode = OpenMode.CREATE
        };

        using var writer = new IndexWriter(_directory, config);

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var translations = await db.TranslationTexts
            .AsNoTracking()
            .Select(t => new { t.Id, t.Text })
            .ToListAsync();

        foreach (var t in translations)
        {
            var doc = new Document
            {
                new StringField(LuceneSchema.FieldType, LuceneSchema.TypeTranslation, Field.Store.YES),
                new StringField(LuceneSchema.FieldId, t.Id.ToString(), Field.Store.YES),
                new TextField(LuceneSchema.FieldText, t.Text ?? string.Empty, Field.Store.YES)
            };
            writer.UpdateDocument(new Term(LuceneSchema.FieldId, t.Id.ToString()), doc);
        }

        var sections = await db.Sections
            .AsNoTracking()
            .Include(s => s.Meanings).ThenInclude(m => m.Language)
            .ToListAsync();

        foreach (Section s in sections)
        {
            foreach (SectionMeaning m in s.Meanings)
            {
                var doc = new Document
                {
                    new StringField(LuceneSchema.FieldType, LuceneSchema.TypeSection, Field.Store.YES),
                    new StringField(LuceneSchema.FieldId, s.Id.ToString(), Field.Store.YES),
                    new TextField(LuceneSchema.FieldName, m.Meaning ?? string.Empty, Field.Store.YES)
                };
                writer.AddDocument(doc);
            }
        }

        writer.Flush(triggerMerge: false, applyAllDeletes: false);
    }
}
