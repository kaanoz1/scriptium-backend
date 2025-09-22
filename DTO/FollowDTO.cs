using ScriptiumBackend.Models;
/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.


namespace DTO
{
    public abstract class FollowUserDto
    {
        public required DateTime OccurredAt { get; set; }
    }

    public class FollowerUserDto : FollowUserDto
    {
        public required UserDto FollowerUser { get; set; }
    }

    public class FollowedUserDto : FollowUserDto
    {
        public required UserDto FollowedUser { get; set; }

    }

    public static class FollowUserDtoExtensions
    {
        public static FollowerUserDto ToFollowerUserDto(this Follow follow)
        {
            return new FollowerUserDto
            {
               OccurredAt = follow.OccurredAt,
               FollowerUser = follow.Follower.ToUserDto(),
            };
        }

        public static FollowedUserDto ToFollowedUserDto(this Follow follow)
        {
            return new FollowedUserDto
            { 
                OccurredAt = follow.OccurredAt,
                FollowedUser = follow.Followed.ToUserDto(),
            };
        }
    }
}

*/