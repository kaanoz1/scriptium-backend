using ScriptiumBackend.Models;
/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.



namespace DTO
{
    public class BlockDto
    {

        public required UserDto BlockedUser { get; set; }
        public required DateTime BlockedAt { get; set; }
        public string? Reason { get; set; }
    }
    public static class BlockExtension
    {
        public static BlockDto ToBlockDto(this Block block)
        {
            ArgumentNullException.ThrowIfNull(block);

            if (block.Blocked is null)
                    throw new ArgumentNullException($"Blocked user cannot be null {block.Id}");

            return new BlockDto
                {
                    BlockedUser = block.Blocked.ToUserDto(),
                    Reason = block.Reason,
                    BlockedAt = block.BlockedAt,
                };

        }


    }
}
*/