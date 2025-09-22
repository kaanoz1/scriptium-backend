using FluentValidation;


namespace ScriptiumBackend.Controllers.Validation
{

    public class RootValidatedDto
    {
        public required short ScriptureNumber { get; set; }
        public required string RootLatin { get; set; }
    }
    public class RootValidator : AbstractValidator<RootValidatedDto>
    {
        public RootValidator()
        {
            RuleFor(r => r.ScriptureNumber)
            .Cascade(CascadeMode.Stop)
            .Must(number => number >= 1)
            .WithMessage("Scripture number is too small; minimum is 1.")
            .Must(Utility.SCRIPTURE_DATA.ContainsKey)
            .WithMessage(r => $"Scripture number {r.ScriptureNumber} is not valid.");
            RuleFor(r => r.RootLatin).MinimumLength(Utility.MIN_LENGTH_FOR_ROOT).MaximumLength(Utility.MAX_LENGTH_FOR_ROOT);

        }

    }
}