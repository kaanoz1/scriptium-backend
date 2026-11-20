namespace ScriptiumBackend.Model.Derived.Islam.Hadith.ArabicOnly;

/// <summary>
/// Foreign-key column names of the hadith tables. The models expose only
/// navigations; these columns are EF shadow properties. Used by the
/// <c>[ForeignKey]</c> attributes, the Fluent configuration and by queries
/// that filter on a key without joining the referenced table
/// (<c>EF.Property&lt;int&gt;(hadith, ForeignKeys.Book)</c>).
/// </summary>
public static class ForeignKeys
{
    public const string Category = "i_h_ao_book_category_id";
    public const string Book = "i_h_ao_book_id";
    public const string MeaningGroup = "i_h_ao_meaning_group_id";
    public const string Hadith = "i_h_ao_hadith_id";
    public const string Sanad = "i_h_ao_sanad_id";
    public const string Narrator = "i_h_ao_narrator_id";
    public const string Scholar = "i_h_ao_scholar_id";
    public const string ParentTopic = "parent_id";
    public const string Teacher = "teacher_id";
    public const string Student = "student_id";
}
