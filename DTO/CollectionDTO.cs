using ScriptiumBackend.Models;
/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.


namespace DTO
{
    public enum CollectionStatus
    {
        Error = 0,
        NotFound = 1,
        AlreadyDone = 2,
        Succeed = 3
    }
    public class CollectionProcessResultDto
    {
        public required string CollectionName { get; set; }
        public required int Code { get; set; }
        public string? Message { get; set; }
    }

    public class CollectionDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string? Description { get; set; }
        public required int SaveCount { get; set; }
    }

    public class CollectionWithVerseSavedInformationDto
    {
        public required string Name { get; set; }
        public required string? Description { get; set; }
        public required bool IsSaved { get; set; }
    }

    public static class CollectionInsertResultDtoExtension
    {
        public static CollectionDto ToCollectionDto(this Collection Collection)
        {
            return new CollectionDto
            {
                Id = Collection.Id,
                Name = Collection.Name,
                Description = Collection.Description,
                SaveCount = Collection.Verses?.Count ?? 0
            };
        }

        public static CollectionProcessResultDto GetCollectionProcessResultDto(this Collection? collection, string CollectionName)
        {
            return new CollectionProcessResultDto
            {
                CollectionName = CollectionName,
                Code = 404,
                Message = $"Collection named '{CollectionName}' not found."
            };

        }

        public static CollectionProcessResultDto GetCollectionProcessResultDto(this Collection collection, CollectionStatus status)
        {
            if (status == CollectionStatus.Succeed)
                return new CollectionProcessResultDto
                {
                    CollectionName = collection.Name,
                    Code = 200,
                    Message = $"The verse has been successfully removed from collection named '{collection.Name}'."
                };

            else if (status == CollectionStatus.AlreadyDone)
                return new CollectionProcessResultDto
                {
                    CollectionName = collection.Name,
                    Code = 409,
                    Message = $"Collection '{collection.Name}' has already conform demanded situation."

                };

            else if (status == CollectionStatus.NotFound)
                return new CollectionProcessResultDto
                {
                    CollectionName = collection.Name,
                    Code = 409,
                    Message = $"Content not found in collection '{collection.Name}'."
                };

            else //Error case, which is more likely to not be.
                return new CollectionProcessResultDto
                {
                    CollectionName = collection.Name,
                    Code = 500,
                    Message = $"An unexpected error has been occurred while saving to collection '{collection.Name}'."
                };



        }

    }
}
*/