using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace ScriptiumBackend.Validation
{
    public class TranslationTextIdentifierModel
    {
        public required long TranslationTextId { get; set; }
    }

    public class TranslationTextIdentifierModelValidator : AbstractValidator<TranslationTextIdentifierModel>
    {
        public TranslationTextIdentifierModelValidator()
        {
            RuleFor(r => r.TranslationTextId)
                        .GreaterThan(0).WithMessage("Variable TranslationTextId must be greater than 0.");

        }
    }
}