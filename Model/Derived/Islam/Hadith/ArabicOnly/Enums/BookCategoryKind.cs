namespace ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;

/// <summary>
/// Genre (تصنيف) of a hadith book. The values are the source's
/// <c>tasnif_no</c> and the primary keys of <see cref="BookCategory"/>, which
/// keeps the Arabic name. Stored as <c>smallint</c>.
/// </summary>
public enum BookCategoryKind : short
{
    /// <summary>صحاح ومستخرجات - Sahih collections and their mustakhrajs.</summary>
    SahihAndMustakhraj = 1,

    /// <summary>سنن وموطآت - Sunan and Muwatta works.</summary>
    SunanAndMuwatta = 2,

    /// <summary>مسانيد - arranged by companion.</summary>
    Musnad = 3,

    /// <summary>مصنفات وجوامع - Musannafs and comprehensive collections.</summary>
    MusannafAndJami = 4,

    /// <summary>معاجم ومشيخات - arranged by teacher.</summary>
    MujamAndMashyakha = 5,

    /// <summary>أجزاء وفوائد - small treatises and notes.</summary>
    JuzAndFawaid = 6,

    /// <summary>أمالي - dictation sessions.</summary>
    Amali = 7,

    /// <summary>علوم الحديث - hadith sciences.</summary>
    HadithSciences = 8,

    /// <summary>عقيدة - creed.</summary>
    Aqidah = 9,

    /// <summary>فقه - jurisprudence.</summary>
    Fiqh = 10,

    /// <summary>تفسير قرآن - Qur'an exegesis.</summary>
    Tafsir = 11,

    /// <summary>علوم قرآن - Qur'anic sciences.</summary>
    QuranSciences = 12,

    /// <summary>سيرة وشمائل - biography and character of Muhammad.</summary>
    SirahAndShamail = 13,

    /// <summary>تراجم وتواريخ - biographical dictionaries and histories.</summary>
    BiographyAndHistory = 14,

    /// <summary>مناقب الصحابة - virtues of the companions.</summary>
    Manaqib = 15,

    /// <summary>زهد ورقائق - asceticism and heart-softeners.</summary>
    ZuhdAndRaqaiq = 16,

    /// <summary>أخلاق - ethics.</summary>
    Akhlaq = 17,

    /// <summary>موضوعات - collections of fabricated reports.</summary>
    Mawduat = 18,
}
