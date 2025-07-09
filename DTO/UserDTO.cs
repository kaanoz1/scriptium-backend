using scriptium_backend_dotnet.Models;
using scriptium_backend_dotnet.Models.Util;

namespace DTO
{
    public class UserDTO
    {
        public required Guid Id { get; set; }

        public required string Username { get; set; }

        public byte[]? Image { get; set; }

        public required string Name { get; set; }
        public string? Surname { get; set; }
        public required bool IsFrozen { get; set; } = false;
    }

    public class UserFetchedDTO
    {
        public required Guid Id { get; set; }

        public required string Username { get; set; }

        public required string Name { get; set; }

        public required string? Surname { get; set; }

        public required string? Biography { get; set; }

        public required byte[]? Image { get; set; }

        public required long FollowerCount { get; set; }

        public required long FollowedCount { get; set; }

        public required long ReflectionCount { get; set; }

        public required long NoteCount { get; set; }

        public required long SuggestionCount { get; set; }

        public required DateTime? PrivateFrom { get; set; }

        public required List<string> Roles { get; set; }

        public required DateTime CreatedAt { get; set; }

        public required int UpdateCount { get; set; }

        public required bool IsFrozen { get; set; } = false;

        public required string? followStatusUserInspected { get; set; }

        public required string? followStatusUserInspecting { get; set; }

        public required bool IsUserInspectedBlocked { get; set; }

    }

    public class UserOwnDTO
    {
        public required Guid Id { get; set; }

        public required string Username { get; set; }

        public required string Name { get; set; }

        public required string? Surname { get; set; }

        public required byte[]? Image { get; set; }

        public required string Email { get; set; }

        public required string? Biography { get; set; }

        public required int LangId { get; set; }

        public required DateTime? PrivateFrom { get; set; }

        public required DateTime CreatedAt { get; set; }

        public required string? Gender { get; set; }

        public required List<string> Roles { get; set; }

    }


    public static class UserDTOExtension
    {

        public static UserDTO ToUserDTO(this User user)
        {
            bool isFrozen = user.IsFrozen.HasValue;

            return new UserDTO
            {
                Id = user.Id,
                Username = user.UserName,
                Name = isFrozen ? string.Empty : user.Name,
                Surname = isFrozen ? string.Empty : user.Surname,
                Image = isFrozen ? null : user.Image,
                IsFrozen = user.IsFrozen.HasValue
            };

        }
    }
}