using System.Text;

namespace ScriptiumBackend.Utils.Text;

/// <summary>
/// Folds Arabic text to a search-friendly form. MUST stay identical to
/// <c>normalize_arabic</c> in scriptium-data-loader (backend/src/ingest/hadith/normalize.rs),
/// which fills the indexed <c>search_text</c> / <c>search_name</c> columns.
/// <list type="bullet">
/// <item>removes harakat, Qur'anic annotation marks and tatweel</item>
/// <item>أ إ آ ٱ → ا, ى → ي, ة → ه, ؤ → و, ئ → ي</item>
/// <item>kept: Arabic letters (U+0621–U+064A, U+0671–U+06D3), Arabic-Indic digits, ASCII letters
/// (lower-cased) and digits; everything else becomes a space and runs of spaces collapse</item>
/// </list>
/// </summary>
public static class ArabicNormalizer
{
    public static string Normalize(string input) => NormalizeWithMap(input).Text;

    /// <summary>
    /// Like <see cref="Normalize"/>, but also returns, for every character of the
    /// result, the index of the character it came from in <paramref name="input"/>
    /// (spaces map to the first character after them).
    /// </summary>
    public static (string Text, int[] Map) NormalizeWithMap(string input)
    {
        var map = new List<int>(input.Length);
        var sb = new StringBuilder(input.Length);
        var pendingSpace = false;

        for (var i = 0; i < input.Length; i++)
        {
            var ch = input[i];
            if (IsDiacritic(ch)) continue;

            var c = Fold(ch);

            if (IsKept(c))
            {
                if (pendingSpace && sb.Length > 0)
                {
                    sb.Append(' ');
                    map.Add(i);
                }

                pendingSpace = false;
                sb.Append(c is >= 'A' and <= 'Z' ? (char)(c + 32) : c);
                map.Add(i);
            }
            else
            {
                pendingSpace = true;
            }
        }

        return (sb.ToString(), map.ToArray());
    }

    public static bool IsDiacritic(char c) =>
        c is >= 'ً' and <= 'ٟ' // harakat, tanwin, shadda, sukun, ...
            or 'ٰ'                  // superscript alef
            or >= 'ۖ' and <= 'ۭ' // Qur'anic annotation marks
            or 'ـ';                 // tatweel

    private static bool IsKept(char c) =>
        c is >= '\u0621' and <= '\u064A'
            or >= '\u0671' and <= '\u06D3'
            or >= '\u0660' and <= '\u0669'
            or >= '\u06F0' and <= '\u06F9'
            or >= 'a' and <= 'z'
            or >= 'A' and <= 'Z'
            or >= '0' and <= '9';

    private static char Fold(char c) => c switch
    {
        'أ' or 'إ' or 'آ' or 'ٱ' => 'ا',
        'ى' => 'ي',
        'ة' => 'ه',
        'ؤ' => 'و',
        'ئ' => 'ي',
        _ => c,
    };
}
