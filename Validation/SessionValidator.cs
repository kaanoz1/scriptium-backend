using FluentValidation;
using SixLabors.ImageSharp;

namespace scriptium_backend_dotnet.Controllers.Validation
{
    public class UpdateProfileModel
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Username { get; set; }
        public string? Biography { get; set; }
        public string? Gender { get; set; }
        public byte? LanguageId { get; set; }
        public IFormFile? Image { get; set; }
    }

    public class UpdateProfileValidator : AbstractValidator<UpdateProfileModel>
{
    private readonly long _maxFileSize = 8 * 1024 * 1024; // 8MB
    private readonly int _requiredDimension = 1024;

    public UpdateProfileValidator()
    {
        RuleFor(x => x)
            .Must(AtLeastOneFieldProvided)
            .WithMessage("At least one field must be provided for update.");

        RuleFor(x => x.Name)
            .MaximumLength(16).WithMessage("Name cannot exceed 16 characters.")
            .Must(NotOnlyWhitespace).WithMessage("Name cannot consist of whitespace.")
            .When(x => x.Name != null);

        RuleFor(x => x.Surname)
            .MaximumLength(16).WithMessage("Surname cannot exceed 16 characters.")
            .Must(NotOnlyWhitespace).WithMessage("Surname cannot consist of whitespace.")
            .When(x => x.Surname != null);

        RuleFor(x => x.Username)
            .MinimumLength(5).WithMessage("Username must be at least 5 characters long.")
            .MaximumLength(16).WithMessage("Username cannot exceed 16 characters.")
            .Matches("^[a-zA-Z0-9._]*$").WithMessage("Username can only contain letters, numbers, dots, and underscores.")
            .Must(NotOnlyWhitespace).WithMessage("Username cannot consist of whitespace.")
            .When(x => x.Username != null);

        RuleFor(x => x.Biography)
            .MaximumLength(200).WithMessage("Biography cannot exceed 200 characters.")
            .Must(NotOnlyWhitespace).WithMessage("Biography cannot consist of whitespace.")
            .When(x => x.Biography != null);

        RuleFor(x => x.Gender)
            .MaximumLength(1).WithMessage("Gender must be a single character.")
            .Must(g => string.IsNullOrEmpty(g) || g == "M" || g == "F" || g == "O")
            .WithMessage("Invalid gender. Allowed values are 'M', 'F', or 'O'.");

        RuleFor(x => x.LanguageId)
            .GreaterThanOrEqualTo((byte)1).WithMessage("Language ID must be a valid positive number.")
            .When(x => x.LanguageId.HasValue);

        RuleFor(x => x.Image)
            .Must(IsAllowedExtension).WithMessage("Only JPEG or JPG files are allowed.")
            .Must(f => f != null && f.Length <= _maxFileSize).WithMessage($"Image size must be less than {_maxFileSize / (1024 * 1024)} MB.")
            .Must(IsValidImage).WithMessage($"Image must be {_requiredDimension}x{_requiredDimension} pixels and square.")
            .When(x => x.Image != null);
    }

    private bool AtLeastOneFieldProvided(UpdateProfileModel model)
    {
        return !string.IsNullOrWhiteSpace(model.Name) ||
               !string.IsNullOrWhiteSpace(model.Surname) ||
               !string.IsNullOrWhiteSpace(model.Username) ||
               !string.IsNullOrWhiteSpace(model.Biography) ||
               !string.IsNullOrWhiteSpace(model.Gender) ||
               model.LanguageId.HasValue ||
               model.Image is not null;
    }

    private bool NotOnlyWhitespace(string? input)
    {
        return string.IsNullOrWhiteSpace(input) == false;
    }

    private bool IsAllowedExtension(IFormFile? file)
    {
        if (file == null) return false;
        string[] allowedExtensions = [".jpg", ".jpeg"];
        string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return allowedExtensions.Contains(extension);
    }

    private bool IsValidImage(IFormFile? file)
    {
        if (file == null) return false;
        try
        {
            using var stream = file.OpenReadStream();
            using var image = Image.Load(stream);
            return image.Width == image.Height && image.Width == _requiredDimension;
        }
        catch
        {
            return false;
        }
    }
}

    public class ChangePasswordModel
    {
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }

    public class ChangePasswordValidator : AbstractValidator<ChangePasswordModel>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty().WithMessage("Old password is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
        }
    }

    public class PasswordModel
    {
        public required string Password { get; set; }
    }

    public class PasswordModelValidator : AbstractValidator<PasswordModel>
    {
        public PasswordModelValidator()
        {

            RuleFor(x => x.Password).AuthenticationPasswordRules();

        }
    }
    
    public class ChangeEmailModel
    {
        public required string NewEmail { get; set; }
        public required string Password { get; set; }
    }


    public class ChangeEmailModelValidator : AbstractValidator<ChangeEmailModel>
    {
        public ChangeEmailModelValidator()
        {
            RuleFor(x => x.NewEmail)
                .AuthenticationEmailRules();

            RuleFor(x => x.Password).AuthenticationPasswordRules();
        }
    }

}