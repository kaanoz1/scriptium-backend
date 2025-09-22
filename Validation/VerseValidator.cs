using FluentValidation;


namespace ScriptiumBackend.Controllers.Validation
{
    public class VerseValidatedModel
    {
        public required short ScriptureNumber { get; set; }
        public required short SectionNumber { get; set; }
        public required short ChapterNumber { get; set; }
        public required int VerseNumber { get; set; }
    }

    public class VerseValidator : AbstractValidator<VerseValidatedModel>
    {
        public VerseValidator()
        {
            RuleFor(x => x.ScriptureNumber)
             .ScriptureNumberRules();

            RuleFor(x => x.SectionNumber)
                .SectionNumberRules(
                    model => model.ScriptureNumber
                );

            RuleFor(x => x.ChapterNumber)
                .ChapterNumberRules(
                    model => model.ScriptureNumber,
                    model => model.SectionNumber
                );

            RuleFor(x => x.VerseNumber)
                .VerseNumberRules(
                    model => model.ScriptureNumber,
                    model => model.SectionNumber,
                    model => model.ChapterNumber
                );
        }


    }

    public class ScriptureValidatedModel
    {
        public required short ScriptureNumber { get; set; }
    }

    public class ScriptureValidator : AbstractValidator<ScriptureValidatedModel>
    {
        public ScriptureValidator()
        {
            RuleFor(x => x.ScriptureNumber).ScriptureNumberRules();
        }
    }

    public class SectionValidatedModel
    {
        public required short ScriptureNumber { get; set; }
        public required short SectionNumber { get; set; }

    }
    public class SectionValidator : AbstractValidator<SectionValidatedModel>
    {
        public SectionValidator()
        {
            RuleFor(x => x.ScriptureNumber)
                  .ScriptureNumberRules();


            RuleFor(x => x.SectionNumber)
                .SectionNumberRules(model => model.ScriptureNumber);
        }
    }

    public class ChapterValidatedModel
    {
        public required short ScriptureNumber { get; set; }
        public required short SectionNumber { get; set; }
        public required short ChapterNumber { get; set; }

    }
    public class ChapterValidator : AbstractValidator<ChapterValidatedModel>
    {
        public ChapterValidator()
        {
            RuleFor(x => x.ScriptureNumber)
                  .ScriptureNumberRules();


            RuleFor(x => x.SectionNumber)
                .SectionNumberRules(x => x.ScriptureNumber);

            RuleFor(x => x.ChapterNumber)
                .ChapterNumberRules(x => x.ScriptureNumber, x => x.SectionNumber);
        }
    }

}
