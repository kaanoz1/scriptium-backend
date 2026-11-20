namespace ScriptiumBackend.Controllers.Islam.JarhTadil.ArabicOnly;

/// <summary>
/// Route prefixes of the jarh wa ta'dil (narrator criticism) corpora.
/// <para>
/// The Arabic-only corpus reads the narrator and critic tables that come with
/// the Arabic hadith corpus (<c>i_h_ao_*</c>): the chains reference the same
/// narrators. Translated jarh wa ta'dil works can get their own segment.
/// </para>
/// </summary>
public static class JarhTadilRoutes
{
    public const string ArabicOnly = "api/islam/jarh-tadil/ao";
}
