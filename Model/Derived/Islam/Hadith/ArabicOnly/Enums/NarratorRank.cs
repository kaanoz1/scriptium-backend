namespace ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly.Enums;

/// <summary>
/// Reliability scale of narrators (مرتبة الراوي), best to worst, used for
/// narrators and for the critics' own standing. Stored as <c>smallint</c>.
/// The verbal judgement (e.g. "ثقة حافظ") is kept separately as text.
/// </summary>
public enum NarratorRank : short
{
    /// <summary>Not graded by the source.</summary>
    NotGraded = 0,

    /// <summary>صحابي - companion of Prophet Muhammad.</summary>
    Companion = 1,

    /// <summary>ثقة حافظ / ثقة ثبت - highest grades of reliability.</summary>
    HighlyReliable = 2,

    /// <summary>ثقة - reliable.</summary>
    Reliable = 3,

    /// <summary>صدوق - truthful.</summary>
    Truthful = 4,

    /// <summary>صدوق يهم / يخطئ - truthful but errs.</summary>
    TruthfulWithErrors = 5,

    /// <summary>مقبول - acceptable when corroborated.</summary>
    Acceptable = 6,

    /// <summary>مجهول الحال - known person, unknown reliability.</summary>
    UnknownCondition = 7,

    /// <summary>ضعيف - weak.</summary>
    Weak = 8,

    /// <summary>مجهول - unknown person.</summary>
    Unknown = 9,

    /// <summary>متروك - abandoned.</summary>
    Abandoned = 10,

    /// <summary>متهم - suspected (of lying or forgery) by the critics.</summary>
    Suspected = 11,

    /// <summary>
    /// كذاب / وضاع - attributed with lying or fabrication by the critics.
    /// This reports the critics' verdict as recorded by the source; it is
    /// not the site's own judgement of the person.
    /// </summary>
    AttributedWithLying = 12,
}
