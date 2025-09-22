using ScriptiumBackend.Models;

namespace DTO
{
    public abstract class WordBaseDto
    {
        public required long Id { get; set; }

        public required short SequenceNumber { get; set; }
       

    }

    public abstract class WordSimpleDto : WordBaseDto
    {
        public required TextVariation Variation { get; set; }
        public required List<WordMeaningDto> Meanings { get; set; } = [];
    }

    public class WordDto: WordSimpleDto;

    public class WordUpperDto : WordDto
    {
        public required VerseUpperDto Verse { get; set; }
    }

    public class WordOneLevelUpperDto : WordDto
    {

        public required VerseDto Verse { get; set;}
    }


    public class WordLowerDto : WordDto
    {
        public List<RootDto> Roots { get; set; } = [];
    }

    public class WordBothDto : WordDto
    {
        public required VerseDto Verse { get; set; }

        public List<RootDto> Roots { get; set; } = [];

    }

    public class WordMeaningDto: Meaning;

    public abstract class WordConfinedDto : WordBaseDto;

    public class WordUpperConfinedDto : WordConfinedDto
    {
        public required VerseUpperConfinedDto Verse { get; set; }
    }
    public class WordLowerConfinedDto : WordConfinedDto
    {
        public required List<RootLowerConfinedDto> Roots { get; set; }
    }
    public static class WordExtensions
    {

        public static WordDto ToWordDto(this Word word)
        {
            return new WordDto
            {
                Id = word.Id,
                SequenceNumber = word.SequenceNumber,
                Variation = new TextVariation { Usual = word.Text, Simplified = word.TextSimplified, WithoutVowel = word.TextWithoutVowel},
                Meanings = word.WordMeanings.Select(wm => wm.ToWordMeaningDto()).ToList()
            };
        }
        public static WordLowerDto ToWordLowerDto(this Word word)
        {
            return new WordLowerDto
            {
                Id = word.Id,
                SequenceNumber = word.SequenceNumber,
                Variation = new TextVariation { Simplified = word.TextSimplified, Usual = word.Text, WithoutVowel = word.TextWithoutVowel },
                Roots = word.Roots.Select(r => r.ToRootDto()).ToList(),
                Meanings = word.WordMeanings.Select(wm => wm.ToWordMeaningDto()).ToList()
            };
        }

        public static WordUpperDto ToWordUpperDto(this Word word)
        {
            return new WordUpperDto()
            {
                Id = word.Id,
                SequenceNumber = word.SequenceNumber,
                Variation = new TextVariation { Simplified = word.TextSimplified, Usual = word.Text, WithoutVowel = word.TextWithoutVowel },
                Meanings = word.WordMeanings.Select(wm => wm.ToWordMeaningDto()).ToList(),
                Verse = word.Verse.ToVerseUpperDto()
            };
        }

        public static WordUpperConfinedDto ToWordUpperConfinedDto(this Word word)
        {
            return new WordUpperConfinedDto
            {
                Id = word.Id,
                SequenceNumber = word.SequenceNumber,
                Verse = word.Verse.ToVerseUpperConfinedDto(),

            };
        }

        public static WordLowerConfinedDto ToWordLowerConfinedDto(this Word word)
        {
            return new WordLowerConfinedDto
            {
                Id = word.Id,
                SequenceNumber = word.SequenceNumber,
                Roots = word.Roots.Select(r => r.ToRootLowerConfinedDto()).ToList(),
            };
        }

        public static WordMeaningDto ToWordMeaningDto(this WordMeaning meaning)
        {

            return new WordMeaningDto
            {
                Language = meaning.Language.ToLanguageDto(),
                Text = meaning.Meaning
            };
        }

      
    }
}