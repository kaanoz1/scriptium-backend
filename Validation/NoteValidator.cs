using FluentValidation;
using Ganss.Xss;
using HtmlAgilityPack;

namespace scriptium_backend_dotnet.Controllers.Validation
{
    public class NoteIdentifierModel
    {
        public required long NoteId { get; set; }
    }

    public class NoteIdentifierModelValidator : AbstractValidator<NoteIdentifierModel>
    {
        public NoteIdentifierModelValidator()
        {
            RuleFor(r => r.NoteId)
            .GreaterThan(0).WithMessage("NoteId must be greater than 0.");
        }
    }
    public class NoteUpdateModel
    {

        public required long NoteId { get; set; }
        public required string NoteText { get; set; }
    }

    public class NoteUpdateModelValidator : AbstractValidator<NoteUpdateModel>
    {
        public NoteUpdateModelValidator()
        {
            RuleFor(r => r.NoteId)
           .GreaterThan(0).WithMessage("Variable NoteId must be greater than 0.");
            RuleFor(r => r.NoteText).NoteTextRules();
        }

    }

    public class NoteCreateModel
    {

        public required string NoteText { get; set; }
        public required int VerseId { get; set; }
    }

    public class NoteCreateModelValidator : AbstractValidator<NoteCreateModel>
    {
        public NoteCreateModelValidator()
        {
            RuleFor(r => r.NoteText).NoteTextRules();
            RuleFor(r => r.VerseId).GreaterThan(0).LessThan(int.MaxValue).WithMessage("VerseId should be valid.");
        }

    }
}