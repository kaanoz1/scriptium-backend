using Microsoft.EntityFrameworkCore;

namespace ScriptiumBackend.Db.Configurations.Islam;

/// <summary>
/// Entry point for every Islam-related part of the schema. Add new Islamic
/// corpora (tafsir, translated hadith collections, ...) here.
/// </summary>
public static class IslamConfiguration
{
    public static void Configure(ModelBuilder modelBuilder)
    {
        QuranConfiguration.Configure(modelBuilder);
        HadithArabicOnlyConfiguration.Configure(modelBuilder);
    }
}
