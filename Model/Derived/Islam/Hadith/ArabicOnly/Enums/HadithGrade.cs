namespace ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;

/// <summary>
/// Grade (حكم) of a single narration (hadith row). Codes 0-5 mean the same as
/// <see cref="ChainGrade"/>; the source gives reports that don't reach
/// Muhammad, or have no chain, their own codes instead of a soundness grade.
/// Stored as <c>smallint</c>.
/// </summary>
public enum HadithGrade : short
{
    /// <summary>بدون سند - quoted without a chain.</summary>
    NoChain = -1,

    /// <summary>صحيح</summary>
    Sahih = 0,

    /// <summary>حسن</summary>
    Hasan = 1,

    /// <summary>ضعيف</summary>
    Daif = 2,

    /// <summary>شديد الضعف</summary>
    VeryWeak = 3,

    /// <summary>متهم بالوضع</summary>
    AccusedOfForgery = 4,

    /// <summary>موضوع</summary>
    Mawdu = 5,

    /// <summary>موقوف - the words of a companion; not graded for soundness.</summary>
    Mawquf = 6,

    /// <summary>مقطوع - the words of a successor or later; not graded for soundness.</summary>
    Maqtu = 7,

    /// <summary>بدون شواهد أو سند - neither a chain nor corroboration.</summary>
    NoSupport = 8,
}
