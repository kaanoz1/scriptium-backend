using System.Data;
using FluentValidation;

namespace scriptium_backend_dotnet.Controllers.Validation
{
    public class RegisterModel
    {
        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Surname { get; set; }

        public string? Gender { get; set; }
        public IFormFile? Image { get; set; }
    }
    public class AuthValidator : AbstractValidator<RegisterModel>
    {
        private readonly long _maxFileSize = 8 * 1024 * 1024; //8MB
        private readonly long _requiredHeight = 1024;
        private readonly long _requiredWidth = 1024;

        public AuthValidator()
        {
            RuleFor(r => r.Username).AuthenticationUsernameRules();
            RuleFor(r => r.Email).AuthenticationEmailRules();
            RuleFor(r => r.Password).AuthenticationPasswordRules();
            RuleFor(r => r.Name).AuthenticationNameRules();
            RuleFor(r => r.Surname).AuthenticationSurnameRules();
            RuleFor(r => r.Gender).AuthenticationGenderRules();
            RuleFor(r => r.Image).AuthenticationImageRules(_maxFileSize, _requiredWidth, _requiredHeight);
        }

    }

    public class LoginModel
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
    }

    public class LoginValidator : AbstractValidator<LoginModel>
    {
        public LoginValidator()
        {
            RuleFor(r => r.Email).AuthenticationEmailRules();

            RuleFor(l => l.Password).AuthenticationPasswordRules();
            
            RuleFor(r => r.RememberMe).NotNull();
        }
    }
    
    
    public class ResetPasswordModel
    {
        public required string Email { get; set; }
        public required string Token { get; set; }
        public required string NewPassword { get; set; }
    }

    public class ResetPasswordValidator : AbstractValidator<ResetPasswordModel>
    {
        public ResetPasswordValidator()
        {
            RuleFor(r => r.Email).AuthenticationEmailRules();
            RuleFor(r => r.NewPassword).AuthenticationPasswordRules();
        }
    }

}

