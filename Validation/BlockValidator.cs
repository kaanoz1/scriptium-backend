/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.


using FluentValidation;

namespace ScriptiumBackend.Controllers.Validation
{
    public class BlockModel
    {
        public required string UserName { get; set; }
        public string? Reason { get; set; }
    }
    public class BlockModelValidator : AbstractValidator<BlockModel>
    {
        public BlockModelValidator()
        {
            RuleFor(r => r.UserName).AuthenticationUsernameRules();

            RuleFor(r => r.Reason).MaximumLength(100).WithMessage("Reason cannot exceed 100 characters.");

        }
    }
}*/