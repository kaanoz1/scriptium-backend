namespace ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;

/// <summary>Kind of report (نوع الحديث): to whom the words are attributed. Stored as <c>smallint</c>.</summary>
public enum HadithType : short
{
    /// <summary>قدسي - words attributed to God, transmitted through Muhammad.</summary>
    Qudsi = 0,

    /// <summary>مرفوع - attributed to Muhammad himself.</summary>
    Marfu = 1,

    /// <summary>موقوف - stops at a companion.</summary>
    Mawquf = 2,

    /// <summary>مقطوع - stops at a successor (tabi'i) or later.</summary>
    Maqtu = 3,
}
