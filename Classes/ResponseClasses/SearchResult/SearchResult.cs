using ScriptiumBackend.Dto.Derived.Islam.Quranic.VerseTranslation;
using ScriptiumBackend.Interfaces;

namespace ScriptiumBackend.Classes.ResponseClasses.SearchResult;

public class SearchResult : ICacheable
{
    public SearchResult() { }

    public SearchResult(List<WithVerse> verses)
    {
        Verses = verses;
    }

    public List<WithVerse> Verses { get; set; } = [];
}