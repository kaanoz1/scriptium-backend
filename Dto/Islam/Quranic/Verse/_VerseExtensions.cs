namespace ScriptiumBackend.Dto.Islam.Quranic.Verse;

using Model.Islam.Quranic;

public static class VerseExtensions
{
    extension(Verse verse)
    {
        public PlainVerseDto ToPlainVerseDto()
        {
            ArgumentNullException.ThrowIfNull(verse.VerseC);
            
            return new PlainVerseDto()
            {
                Sequence = verse.VerseC.Number,
                Simple = verse.Simple,
                SimpleClean = verse.SimpleClean,
                SimpleMinimal = verse.SimpleMinimal,
                SimplePlain = verse.SimplePlain,
                Uthmani = verse.Uthmani,
                UthmaniMinimal = verse.UthmaniMinimal,
            };
        }
    }
}