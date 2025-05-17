using scriptium_backend_dotnet.Models;

namespace DTO
{
    public class BlockDTO
    {

        public required UserDTO BlockedUser { get; set; }
        public required DateTime BlockedAt { get; set; }
        public string? Reason { get; set; }
    }
    public static class BlockExtension
    {
        public static BlockDTO ToBlockDTO(this Block block)
        {
            return new BlockDTO
            {
                BlockedUser = block.Blocked.ToUserDTO(),
                Reason = block.Reason,
                BlockedAt = block.BlockedAt,
            };

        }


    }
}