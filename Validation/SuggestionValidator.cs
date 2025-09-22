/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.

using FluentValidation;

namespace ScriptiumBackend.Validation
{
    public class SuggestionCreateModel
    {
        public required string SuggestionText { get; set; }
        public required long TranslationTextId { get; set; }
    }

    public class SuggestionCreateModelValidator : AbstractValidator<SuggestionCreateModel>
    {
        public SuggestionCreateModelValidator()
        {
            RuleFor(r => r.SuggestionText)
                         .NotEmpty().WithMessage("SuggestionText is required.")
                         .MaximumLength(500).WithMessage("SuggestionText cannot exceed the limit of 500 characters.");

            RuleFor(r => r.TranslationTextId)
                        .GreaterThan(0).WithMessage("Variable TranslationTextId must be greater than 0.");

        }
    }

    public class SuggestionIdentifierModel
    {
        public required string SuggestionText { get; set; }
        public required long TranslationTextId { get; set; }
    }
}
*/