using scriptium_backend_dotnet.Models;

namespace DTO
{
    public abstract class WordBaseDTO
    {
        public required long Id { get; set; }

        public required short SequenceNumber { get; set; }
       

    }

    public abstract class WordSimpleDTO : WordBaseDTO
    {
        public required TextVariation Variation { get; set; }
        public required List<WordMeaningDTO> Meanings { get; set; } = [];
    }

    public class WordDTO: WordSimpleDTO;

    public class WordUpperDTO : WordDTO
    {
        public required VerseUpperDTO Verse { get; set; }
    }

    public class WordOneLevelUpperDTO : WordDTO
    {

        public required VerseDTO Verse { get; set;}
    }


    public class WordLowerDTO : WordDTO
    {
        public List<RootDTO> Roots { get; set; } = [];
    }

    public class WordBothDTO : WordDTO
    {
        public required VerseDTO Verse { get; set; }

        public List<RootDTO> Roots { get; set; } = [];

    }

    public class WordMeaningDTO: Meaning;

    public abstract class WordConfinedDTO : WordBaseDTO;

    public class WordUpperConfinedDTO : WordConfinedDTO
    {
        public required VerseUpperConfinedDTO Verse { get; set; }
    }
    public class WordLowerConfinedDTO : WordConfinedDTO
    {
        public required List<RootLowerConfinedDTO> Roots { get; set; }
    }
    public static class WordExtensions
    {

        public static WordDTO ToWordDTO(this Word word)
        {
            return new WordDTO
            {
                Id = word.Id,
                SequenceNumber = word.SequenceNumber,
                Variation = new TextVariation { Usual = word.Text, Simplified = word.TextSimplified, WithoutVowel = word.TextWithoutVowel},
                Meanings = word.WordMeanings.Select(wm => wm.ToWordMeaningDTO()).ToList()
            };
        }
        public static WordLowerDTO ToWordLowerDTO(this Word word)
        {
            return new WordLowerDTO
            {
                Id = word.Id,
                SequenceNumber = word.SequenceNumber,
                Variation = new TextVariation { Simplified = word.TextSimplified, Usual = word.Text, WithoutVowel = word.TextWithoutVowel },
                Roots = word.Roots.Select(r => r.ToRootDTO()).ToList(),
                Meanings = word.WordMeanings.Select(wm => wm.ToWordMeaningDTO()).ToList()
            };
        }

        public static WordUpperDTO ToWordUpperDTO(this Word word)
        {
            return new WordUpperDTO()
            {
                Id = word.Id,
                SequenceNumber = word.SequenceNumber,
                Variation = new TextVariation { Simplified = word.TextSimplified, Usual = word.Text, WithoutVowel = word.TextWithoutVowel },
                Meanings = word.WordMeanings.Select(wm => wm.ToWordMeaningDTO()).ToList(),
                Verse = word.Verse.ToVerseUpperDTO()
            };
        }

        public static WordUpperConfinedDTO ToWordUpperConfinedDTO(this Word word)
        {
            return new WordUpperConfinedDTO
            {
                Id = word.Id,
                SequenceNumber = word.SequenceNumber,
                Verse = word.Verse.ToVerseUpperConfinedDTO(),

            };
        }

        public static WordLowerConfinedDTO ToWordLowerConfinedDTO(this Word word)
        {
            return new WordLowerConfinedDTO
            {
                Id = word.Id,
                SequenceNumber = word.SequenceNumber,
                Roots = word.Roots.Select(r => r.ToRootLowerConfinedDTO()).ToList(),
            };
        }

        public static WordMeaningDTO ToWordMeaningDTO(this WordMeaning meaning)
        {

            return new WordMeaningDTO
            {
                Language = meaning.Language.ToLanguageDTO(),
                Text = meaning.Meaning
            };
        }

      
    }
}