using System.Text;

namespace ScriptiumBackend.Utils.Text;

/// <summary>How search terms are matched against the normalised matn.</summary>
public enum ArabicSearchMode
{
    /// <summary>
    /// Every word must occur, with common attached particles (و ف ب ل ك ال …)
    /// allowed in front and any suffix behind (pronouns, plurals).
    /// </summary>
    Smart,

    /// <summary>Every word must occur exactly as typed (no particles, no suffixes).</summary>
    Exact,

    /// <summary>The words must occur consecutively, exactly as typed.</summary>
    Phrase,
}

/// <summary>
/// Builds PostgreSQL <c>tsquery</c> strings for Arabic full-text search and
/// finds the matched spans in the original (diacritised) text.
/// </summary>
public static class ArabicSearch
{
    /// <summary>Particles that attach to the front of a word.</summary>
    private static readonly string[] Proclitics =
        ["و", "ف", "ب", "ل", "ك", "س", "وب", "ول", "فب", "فل", "وك", "فك"];

    public const int MaxTerms = 8;

    /// <summary>Distinct normalised words of the query (at most <see cref="MaxTerms"/>).</summary>
    public static List<string> Terms(string normalizedQuery) =>
        normalizedQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Distinct()
            .Take(MaxTerms)
            .ToList();

    /// <summary>All surface forms a term may take in <see cref="ArabicSearchMode.Smart"/> mode.</summary>
    public static List<string> Variants(string term)
    {
        var stems = new List<string> { term };
        // With or without the article: "صلاه" also finds "الصلاه" and vice versa.
        if (term.StartsWith("ال") && term.Length > 3) stems.Add(term[2..]);
        else stems.Add("ال" + term);

        var variants = new List<string>();
        foreach (var stem in stems)
        {
            variants.Add(stem);
            foreach (var p in Proclitics)
            {
                // ل + ال is written لل.
                if (p.EndsWith('ل') && stem.StartsWith("ال")) variants.Add(p + stem[1..]);
                else variants.Add(p + stem);
            }
        }

        return variants.Distinct().ToList();
    }

    /// <summary>
    /// Builds a <c>to_tsquery('simple', …)</c> string. Terms only contain
    /// characters <see cref="ArabicNormalizer"/> keeps (letters/digits), so they
    /// need no escaping.
    /// </summary>
    public static string BuildTsQuery(IReadOnlyList<string> terms, ArabicSearchMode mode)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < terms.Count; i++)
        {
            if (i > 0) sb.Append(mode == ArabicSearchMode.Phrase ? " <-> " : " & ");

            if (mode == ArabicSearchMode.Smart)
            {
                sb.Append('(');
                sb.Append(string.Join(" | ", Variants(terms[i]).Select(v => v + ":*")));
                sb.Append(')');
            }
            else
            {
                sb.Append(terms[i]);
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// Ranges of <paramref name="original"/> whose words match any term.
    /// Ranges are extended over trailing diacritics so the whole word is marked.
    /// </summary>
    public static List<(int Start, int End)> FindMatches(string original, IReadOnlyList<string> terms,
        ArabicSearchMode mode)
    {
        var (normalized, map) = ArabicNormalizer.NormalizeWithMap(original);
        var forms = mode == ArabicSearchMode.Smart
            ? terms.SelectMany(Variants).ToList()
            : terms.ToList();

        var ranges = new List<(int, int)>();
        var wordStart = 0;
        for (var i = 0; i <= normalized.Length; i++)
        {
            if (i < normalized.Length && normalized[i] != ' ') continue;

            if (i > wordStart)
            {
                var word = normalized[wordStart..i];
                var hit = mode == ArabicSearchMode.Smart
                    ? forms.Any(f => word.StartsWith(f, StringComparison.Ordinal))
                    : forms.Contains(word);

                if (hit)
                {
                    var start = map[wordStart];
                    var end = map[i - 1] + 1;
                    while (end < original.Length && ArabicNormalizer.IsDiacritic(original[end])) end++;
                    ranges.Add((start, end));
                }
            }

            wordStart = i + 1;
        }

        return ranges;
    }
}
