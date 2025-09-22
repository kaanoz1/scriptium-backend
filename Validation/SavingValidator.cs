/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.

using FluentValidation;

namespace ScriptiumBackend.Controllers.Validation
{
    public class SavingProcessModel
    {
        public required int VerseId { get; set; }
        public required List<string> CollectionNames { get; set; }
    }

    public class SavingProcessModelValidator : AbstractValidator<SavingProcessModel>
    {
        public SavingProcessModelValidator()
        {
            RuleFor(r => r.VerseId).Must(v => v < int.MaxValue).WithMessage("VerseId should be valid.");

            RuleFor(r => r.CollectionNames)
            .Must(r => r.Count >= 1)
            .WithMessage("You must specify at least one name of your collections.")
            .Must(r => r.Count <= Utility.MAX_COLLECTION_COUNT)
            .WithMessage($"You must specify at most {Utility.MAX_COLLECTION_COUNT} name of your collections.");

            RuleForEach(r => r.CollectionNames)
                        .CollectionNameRule();

        }

    }
}
*/