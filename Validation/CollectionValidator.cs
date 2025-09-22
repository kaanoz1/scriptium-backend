/*
This file is temporarily disabled. 
Due to budgetary reasons, Scriptium is unable to provide the economic conditions necessary for legal and official procedures involving the collection, processing, and storage of user information.


using FluentValidation;

namespace ScriptiumBackend.Controllers.Validation
{

    public class CollectionCreateModel
    {
        public required string CollectionName { get; set; }
        public string? Description { get; set; }
    }
    public class CollectionCreateModelValidator : AbstractValidator<CollectionCreateModel>
    {
        public CollectionCreateModelValidator()
        {
            RuleFor(r => r.CollectionName)
                        .CollectionNameRule();

            RuleFor(r => r.Description)
                        .CollectionDescriptionRule();

        }
    }

    public class CollectionUpdateModel
    {
        public required int CollectionId { get; set; }
        public string? NewCollectionName { get; set; }
        public string? NewCollectionDescription { get; set; }
    }
    public class CollectionUpdateModelValidator : AbstractValidator<CollectionUpdateModel>
    {
        public CollectionUpdateModelValidator()
        {

            RuleFor(r => r.CollectionId)
                         .CollectionIdRule();


            RuleFor(r => r.NewCollectionName)
                        .CollectionNameRule();

            RuleFor(r => r.NewCollectionDescription)
                        .CollectionDescriptionRule();

        }
    }

    public class CollectionDeleteModel
    {
        public required int CollectionId { get; set; }
    }
    public class CollectionDeleteModelValidator : AbstractValidator<CollectionDeleteModel>
    {
        public CollectionDeleteModelValidator()
        {

            RuleFor(r => r.CollectionId)
                        .CollectionIdRule();


        }
    }
}

*/