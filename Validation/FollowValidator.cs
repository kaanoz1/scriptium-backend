/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.


using FluentValidation;

namespace ScriptiumBackend.Controllers.Validation
{
    public class GetFollowerModel
    {
        public string Type { get; set; } = null!;
    }
    public class TypeValidator : AbstractValidator<GetFollowerModel>
    {
        public TypeValidator()
        {
            RuleFor(Type => Type)
                .NotEmpty()
                .WithMessage("Type cannot be empty.")
                .Must(Model => Model.Type == "Follower" || Model.Type == "Pending")
                .WithMessage("Type must be either 'follower' or 'pending'.");
        }
    }


}
*/