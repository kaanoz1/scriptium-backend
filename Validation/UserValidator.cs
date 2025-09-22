/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.


using FluentValidation;

namespace ScriptiumBackend.Controllers.Validation
{
    public class UserNameModel
    {
        public required string UserName { get; set; }
    }
    public class UserNameValidator : AbstractValidator<UserNameModel>
    {
        public UserNameValidator()
        {
            RuleFor(r => r.UserName)
                         .NotEmpty().WithMessage("Username is required.")
                         .MaximumLength(16).WithMessage("Username cannot exceed 16 characters.");

        }
    }
    public class UserNameMassModel
    {
        public required HashSet<string> UserNames { get; set; }
    }

    public class UserNameMassValidator : AbstractValidator<UserNameMassModel>
    {
        public UserNameMassValidator()
        {
            RuleFor(r => r.UserNames)
                .NotEmpty().WithMessage("At least one username is required.");

            RuleForEach(r => r.UserNames)
                .NotEmpty().WithMessage("Username is required.")
                .MaximumLength(16).WithMessage("Username cannot exceed 16 characters.");

            RuleFor(r => r.UserNames)
                .Must(usernames => usernames.Count == usernames.Distinct().Count())
                .WithMessage("Usernames must be unique.")
                .Must(usernames => usernames.Count < 20)
                .WithMessage("You can unblock at most 20 at once.");
        }
    }

}
*/