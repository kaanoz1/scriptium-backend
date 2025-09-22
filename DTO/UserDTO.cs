
/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.
using ScriptiumBackend.Models;
using ScriptiumBackend.Models.Util;


namespace DTO
{
    public class UserDto
    {
        public required Guid Id { get; set; }

        public required string Username { get; set; }

        public byte[]? Image { get; set; }

        public required string Name { get; set; }
        public string? Surname { get; set; }
        public required bool IsFrozen { get; set; } = false;
    }

    public class UserFetchedDto
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

        // public required int UpdateCount { get; set; }

        public required bool IsFrozen { get; set; } = false;

        public required string? followStatusUserInspected { get; set; }

        public required string? followStatusUserInspecting { get; set; }

        public required bool IsUserInspectedBlocked { get; set; }
    }

    public class UserOwnDto
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


    public static class UserDtoExtension
    {
        public static UserDto ToUserDto(this User user)
        {
            bool isFrozen = user.IsFrozen.HasValue;

            return new UserDto
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

*/