namespace ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;

/// <summary>
/// Grade (حكم) of a chain of narration, a meaning group or a taraf, as given
/// by the source. Stored as <c>smallint</c>.
/// <para>
/// Mostly follows the weakest narrator of the chain, but also depends on the
/// chain being connected: a chain of reliable narrators with a gap (إرسال،
/// تعليق، انقطاع) is graded <see cref="Daif"/>.
/// </para>
/// </summary>
public enum ChainGrade : short
{
    /// <summary>صحيح</summary>
    Sahih = 0,

    /// <summary>حسن</summary>
    Hasan = 1,

    /// <summary>ضعيف</summary>
    Daif = 2,

    /// <summary>شديد الضعف</summary>
    VeryWeak = 3,

    /// <summary>متهم بالوضع - a narrator is accused of forgery.</summary>
    AccusedOfForgery = 4,

    /// <summary>موضوع - fabricated.</summary>
    Mawdu = 5,
}
