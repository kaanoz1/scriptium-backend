using scriptium_backend_dotnet.Models;

namespace DTO
{
    public abstract class FollowUserDTO
    {
        public required DateTime OccurredAt { get; set; }
    }

    public class FollowerUserDTO : FollowUserDTO
    {
        public required UserDTO FollowerUser { get; set; }
    }

    public class FollowedUserDTO : FollowUserDTO
    {
        public required UserDTO FollowedUser { get; set; }

    }

    public static class FollowUserDTOExtensions
    {
        public static FollowerUserDTO ToFollowerUserDTO(this Follow follow)
        {
            return new FollowerUserDTO
            {
               OccurredAt = follow.OccurredAt,
               FollowerUser = follow.Follower.ToUserDTO(),
            };
        }

        public static FollowedUserDTO ToFollowedUserDTO(this Follow follow)
        {
            return new FollowedUserDTO
            { 
                OccurredAt = follow.OccurredAt,
                FollowedUser = follow.Followed.ToUserDTO(),
            };
        }
    }
}