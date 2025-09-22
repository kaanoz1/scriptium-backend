using ScriptiumBackend.Models;

namespace DTO;

public class BookCoverDto
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public List<BookMeaningDto> Meanings { get; set; } = [];
    public string? Description { get; set; }
}

public class BookCoverOneLevelLowerDto : BookCoverDto
{
    public required List<BookNodeCoverDto> Nodes { get; set; }
}

public class BookNodeCoverDto
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public List<BookNodeMeaningDto> Meanings { get; set; } = [];
    public string? Description { get; set; }
}

public class BookNodeOneLevelLowerCoverDto : BookNodeCoverDto
{
    public required List<BookNodeCoverDto> Nodes { get; set; }
}

public class BookNodeOneLevelUpperCoverDto : BookNodeCoverDto
{
    public required BookNodeCoverDto Parent { get; set; }
}

public class BookNodeOneLevelUpperBookCoverDto : BookNodeCoverDto
{
    public required BookCoverDto Book { get; set; }
}

public class BookNodeOneLevelUpperBookAndOneLevelLowerCoverDto : BookNodeCoverDto
{
    public required BookCoverDto Book { get; set; }

    public required List<BookNodeCoverDto> Nodes { get; set; }
}

public class BookNodeOneLevelUpperBookAndOneLevelLowerTextCoverDto : BookNodeCoverDto
{
    public required BookCoverDto Book { get; set; }

    public required List<BookTextDto> Texts { get; set; }
}

public class BookNodeTwoLevelUpperCoverDto : BookNodeCoverDto
{
    public required BookNodeOneLevelUpperCoverDto Parent { get; set; }
}

public class BookNodeTwoLevelUpperBookCoverDto : BookNodeCoverDto
{
    public required BookNodeOneLevelUpperBookCoverDto Parent { get; set; }
}

public class BookNodeThreeLevelUpperCoverDto : BookNodeCoverDto
{
    public required BookNodeTwoLevelUpperCoverDto Parent { get; set; }
}

public class BookNodeThreeLevelUpperBookCoverDto : BookNodeCoverDto
{
    public required BookNodeTwoLevelUpperBookCoverDto Parent { get; set; }
}

public class BookNodeFourLevelUpperBookCoverDto : BookNodeCoverDto
{
    public required BookNodeThreeLevelUpperBookCoverDto Parent { get; set; }
}

public class BookNodeOneLevelUpperBookAndOneLevelLowerDto : BookNodeCoverDto
{
    public required BookCoverDto Book { get; init; }

    public required List<BookNodeCoverDto> Nodes { get; set; }
}

public class BookOneLevelUpperBookAndOneLevelLowerTextCoverDto : BookNodeCoverDto
{
    public required BookCoverDto Book { get; set; }

    public required List<BookTextDto> Texts { get; set; }
}

public class BookNodeTwoLevelUpperBookAndOneLevelLowerDto : BookNodeCoverDto
{
    public required BookNodeOneLevelUpperBookCoverDto Parent { get; set; }

    public required List<BookNodeCoverDto> Nodes { get; set; }
}

public class BookNodeTwoLevelUpperBookAndOneLevelLowerTextCoverDto : BookNodeCoverDto
{
    public required BookNodeOneLevelUpperBookCoverDto Parent { get; set; }

    public required List<BookTextDto> Texts { get; set; }
}

public class BookNodeThreeLevelUpperBookAndOneLevelLowerCoverDto : BookNodeCoverDto
{
    public required BookNodeTwoLevelUpperBookCoverDto Parent { get; set; }

    public required List<BookNodeCoverDto> Nodes { get; set; }
}

public class BookNodeThreeLevelUpperBookAndOneLevelLowerTextCoverDto : BookNodeCoverDto
{
    public required BookNodeTwoLevelUpperBookCoverDto Parent { get; set; }

    public required List<BookTextDto> Texts { get; set; }
}

public class BookNodeFourLevelUpperBookAndOneLevelLowerCoverDto : BookNodeCoverDto
{
    public required BookNodeThreeLevelUpperBookCoverDto Parent { get; set; }

    public required List<BookNodeCoverDto> Nodes { get; set; }
}

public class BookNodeFourLevelUpperBookAndOneLevelLowerTextCoverDto : BookNodeCoverDto
{
    public required BookNodeThreeLevelUpperBookCoverDto Parent { get; set; }

    public required List<BookTextDto> Texts { get; set; }
}



public class BookNodeFiveLevelUpperBookAndOneLevelLowerCoverDto : BookNodeCoverDto
{
    public required BookNodeFourLevelUpperBookCoverDto Parent { get; set; }

    public required List<BookNodeCoverDto> Nodes { get; set; }
}

public class BookNodeFiveLevelUpperBookAndOneLevelLowerTextCoverDto : BookNodeCoverDto
{
    public required BookNodeFourLevelUpperBookCoverDto Parent { get; set; }

    public required List<BookTextDto> Texts { get; set; }
}


public class BookTextDto
{
    public long Id { get; set; }
    public required string Text { get; set; }
    public short SequenceNumber { get; set; }
    public required List<BookTranslationTextDto> TranslationTexts { get; set; }
}

public class BookTranslationTextDto
{
    public required string Text { get; set; }
    public required BookTranslationDto Translation { get; set; }
    
    public required List<BookTranslationTextFootNoteDto> Footnotes { get; set; } 
}

public class BookTranslationTextFootNoteDto
{
    public required string Text { get; set; }
    public required string Indicator { get; set; }
    public int Index { get; set; }
}

public class BookTranslationDto
{
    public short Id { get; set; }
    public required string Name { get; set; }
    public required LanguageDto Language { get; set; }
    public required List<TranslatorDto> Translators { get; set; }
}

public class BookMeaningDto : Meaning;

public class BookNodeMeaningDto : Meaning;

public static class BookExtensions
{
    public static BookCoverDto ToBookCoverDto(this Book book)
    {
        return new BookCoverDto
        {
            Id = book.Id,
            Name = book.Name,
            Description = book.Description,
            Meanings = book.Meanings.Select(m => m.ToBookMeaningDto()).ToList()
        };
    }

    public static BookCoverOneLevelLowerDto ToBookCoverOneLevelLowerDto(this Book book)
    {
        return new BookCoverOneLevelLowerDto
        {
            Id = book.Id,
            Name = book.Name,
            Description = book.Description,
            Meanings = book.Meanings.Select(m => m.ToBookMeaningDto()).ToList(),
            Nodes = book.Nodes.Select(n => n.ToBookNodeCoverDto()).ToList()
        };
    }

    public static BookNodeCoverDto ToBookNodeCoverDto(this BookNode bookNode)
    {
        return new BookNodeCoverDto
        {
            Id = bookNode.Id,
            Name = bookNode.Name,
            Description = bookNode.Description,
            Meanings = bookNode.Meanings.Select(m => m.ToBookNodeMeaningDto()).ToList()
        };
    }

    public static BookNodeOneLevelLowerCoverDto ToBookNodeOneLevelLowerCoverDto(this BookNode bookNode)
    {
        return new BookNodeOneLevelLowerCoverDto
        {
            Id = bookNode.Id,
            Name = bookNode.Name,
            Description = bookNode.Description,
            Meanings = bookNode.Meanings.Select(m => m.ToBookNodeMeaningDto()).ToList(),
            Nodes = bookNode.ChildNodes.Select(cn => cn.ToBookNodeCoverDto()).ToList()
        };
    }

    public static BookNodeOneLevelUpperCoverDto ToBookNodeOneLevelUpperCoverDto(this BookNode bookNode)
    {
        if (bookNode.ParentBookNode is null)
            throw new Exception($"ParentBookNode should not be null. bookNode.Id: {bookNode.Id}");


        return new BookNodeOneLevelUpperCoverDto
        {
            Id = bookNode.Id,
            Name = bookNode.Name,
            Description = bookNode.Description,
            Meanings = bookNode.Meanings.Select(m => m.ToBookNodeMeaningDto()).ToList(),
            Parent = bookNode.ParentBookNode.ToBookNodeCoverDto(),
        };
    }

    public static BookNodeOneLevelUpperBookCoverDto ToBookNodeOneLevelUpperBookCoverDto(this BookNode bookNode)
    {
        if (bookNode.Book is null)
            throw new Exception($"Book should not be null. BookNode.Id: {bookNode.Id}");

        return new BookNodeOneLevelUpperBookCoverDto
        {
            Id = bookNode.Id,
            Name = bookNode.Name,
            Description = bookNode.Description,
            Meanings = bookNode.Meanings.Select(m => m.ToBookNodeMeaningDto()).ToList(),
            Book = bookNode.Book.ToBookCoverDto(),
        };
    }

    public static BookNodeTwoLevelUpperCoverDto ToBookNodeTwoLevelUpperCoverDto(this BookNode bookNode)
    {
        if (bookNode.ParentBookNode is null)
            throw new Exception($"ParentBookNode should not be null. bookNode.Id: {bookNode.Id}");


        return new BookNodeTwoLevelUpperCoverDto
        {
            Id = bookNode.Id,
            Name = bookNode.Name,
            Description = bookNode.Description,
            Meanings = bookNode.Meanings.Select(m => m.ToBookNodeMeaningDto()).ToList(),
            Parent = bookNode.ParentBookNode.ToBookNodeOneLevelUpperCoverDto(),
        };
    }

    public static BookNodeOneLevelUpperBookAndOneLevelLowerDto ToBookOneLevelUpperBookAndOneLevelLowerDto(
        this BookNode bookNode)
    {
        if (bookNode.Book is null)
            throw new Exception($"ParentBookNode should not be null. bookNode.Id: {bookNode.Id}");


        return new BookNodeOneLevelUpperBookAndOneLevelLowerDto
        {
            Id = bookNode.Id,
            Name = bookNode.Name,
            Description = bookNode.Description,
            Meanings = bookNode.Meanings.Select(m => m.ToBookNodeMeaningDto()).ToList(),
            Book = bookNode.Book.ToBookCoverDto(),
            Nodes = bookNode.ChildNodes.Select(n => n.ToBookNodeCoverDto()).ToList(),
        };
    }

    public static BookOneLevelUpperBookAndOneLevelLowerTextCoverDto ToBookOneLevelUpperBookAndOneLevelLowerTextCoverDto(
        this BookNode bookNode)
    {
        if (bookNode.Book is null)
            throw new Exception($"Book should not be null. bookNode.Id: {bookNode.Id}");


        return new BookOneLevelUpperBookAndOneLevelLowerTextCoverDto
        {
            Id = bookNode.Id,
            Name = bookNode.Name,
            Description = bookNode.Description,
            Meanings = bookNode.Meanings.Select(m => m.ToBookNodeMeaningDto()).ToList(),
            Book = bookNode.Book.ToBookCoverDto(),
            Texts = bookNode.Texts.Select(n => n.ToBookTextDto()).ToList(),
        };
    }

    public static BookNodeTwoLevelUpperBookAndOneLevelLowerDto ToBookNodeTwoLevelUpperBookAndOneLevelLowerDto(
        this BookNode bookNode)
    {
        if (bookNode.ParentBookNode is null)
            throw new Exception($"ParentBookNode should not be null. bookNode.Id: {bookNode.Id}");


        return new BookNodeTwoLevelUpperBookAndOneLevelLowerDto
        {
            Id = bookNode.Id,
            Name = bookNode.Name,
            Description = bookNode.Description,
            Meanings = bookNode.Meanings.Select(m => m.ToBookNodeMeaningDto()).ToList(),
            Parent = bookNode.ParentBookNode.ToBookNodeOneLevelUpperBookCoverDto(),
            Nodes = bookNode.ChildNodes.Select(n => n.ToBookNodeCoverDto()).ToList(),
        };
    }

    public static BookNodeTwoLevelUpperBookAndOneLevelLowerTextCoverDto
        ToBookNodeTwoLevelUpperBookAndOneLevelLowerTextCoverDto(
            this BookNode bookNode)
    {
        if (bookNode.ParentBookNode is null)
            throw new Exception($"ParentBookNode should not be null. bookNode.Id: {bookNode.Id}");


        return new BookNodeTwoLevelUpperBookAndOneLevelLowerTextCoverDto
        {
            Id = bookNode.Id,
            Name = bookNode.Name,
            Description = bookNode.Description,
            Meanings = bookNode.Meanings.Select(m => m.ToBookNodeMeaningDto()).ToList(),
            Texts = bookNode.Texts.Select(t => t.ToBookTextDto()).ToList(),
            Parent = bookNode.ParentBookNode.ToBookNodeOneLevelUpperBookCoverDto()
        };
    }

    public static BookTextDto ToBookTextDto(this BookText bookText)
    {
        return new BookTextDto
        {
            Text = bookText.Text,
            Id = bookText.Id,
            SequenceNumber = bookText.SequenceNumber,
            TranslationTexts = bookText.TranslationTexts.Select(tt => tt.ToBookTranslationTextDto()).ToList(),
        };
    }


    public static BookMeaningDto ToBookMeaningDto(this BookMeaning bookMeaning)
    {
        if (bookMeaning.Language is null)
            throw new ArgumentNullException($"Language should not be null. BookMeaningId: {bookMeaning.Id}");


        return new BookMeaningDto
        {
            Language = bookMeaning.Language.ToLanguageDto(),
            Text = bookMeaning.Meaning,
        };
    }

    public static BookNodeMeaningDto ToBookNodeMeaningDto(this BookNodeMeaning bookNodeMeaning)
    {
        if (bookNodeMeaning.Language is null)
            throw new ArgumentNullException($"Language should not be null. BookMeaningId: {bookNodeMeaning.Id}");


        return new BookNodeMeaningDto
        {
            Language = bookNodeMeaning.Language.ToLanguageDto(),
            Text = bookNodeMeaning.Meaning,
        };
    }

    public static BookNodeOneLevelUpperBookAndOneLevelLowerCoverDto ToBookNodeOneLevelUpperBookAndOneLevelLowerCoverDto(
        this BookNode bookNode)
    {
        if (bookNode.Book is null)
            throw new Exception($"Book should not be null. bookNode.Id: {bookNode.Id}");


        return new BookNodeOneLevelUpperBookAndOneLevelLowerCoverDto
        {
            Id = bookNode.Id,
            Name = bookNode.Name,
            Description = bookNode.Description,
            Meanings = bookNode.Meanings.Select(m => m.ToBookNodeMeaningDto()).ToList(),
            Book = bookNode.Book.ToBookCoverDto(),
            Nodes = bookNode.ChildNodes.Select(n => n.ToBookNodeCoverDto()).ToList(),
        };
    }

    public static BookNodeOneLevelUpperBookAndOneLevelLowerTextCoverDto
        ToBookNodeOneLevelUpperBookAndOneLevelLowerTextCoverDto(this BookNode bookNode)
    {
        if (bookNode.Book is null)
            throw new Exception($"Book should not be null. bookNode.Id: {bookNode.Id}");


        return new BookNodeOneLevelUpperBookAndOneLevelLowerTextCoverDto
        {
            Id = bookNode.Id,
            Name = bookNode.Name,
            Description = bookNode.Description,
            Meanings = bookNode.Meanings.Select(m => m.ToBookNodeMeaningDto()).ToList(),
            Book = bookNode.Book.ToBookCoverDto(),
            Texts = bookNode.Texts.Select(t => t.ToBookTextDto()).ToList(),
        };
    }

    public static BookNodeFourLevelUpperBookAndOneLevelLowerCoverDto
        ToBookNodeFourLevelUpperBookAndOneLevelLowerCoverDto(
            this BookNode bookNode)
    {
        if (bookNode.ParentBookNode is null)
            throw new Exception($"ParentBookNode should not be null. bookNode.Id: {bookNode.Id}");


        return new BookNodeFourLevelUpperBookAndOneLevelLowerCoverDto
        {
            Id = bookNode.Id,
            Name = bookNode.Name,
            Description = bookNode.Description,
            Meanings = bookNode.Meanings.Select(m => m.ToBookNodeMeaningDto()).ToList(),
            Parent = bookNode.ParentBookNode.ToBookNodeThreeLevelUpperBookCoverDto(),
            Nodes = bookNode.ChildNodes.Select(n => n.ToBookNodeCoverDto()).ToList(),
        };
    }

    public static BookNodeFourLevelUpperBookAndOneLevelLowerTextCoverDto
        ToBookNodeFourLevelUpperBookAndOneLevelLowerTextCoverDto(
            this BookNode bookNode)
    {
        if (bookNode.ParentBookNode is null)
            throw new Exception($"ParentBookNode should not be null. bookNode.Id: {bookNode.Id}");


        return new BookNodeFourLevelUpperBookAndOneLevelLowerTextCoverDto
        {
            Id = bookNode.Id,
            Name = bookNode.Name,
            Description = bookNode.Description,
            Meanings = bookNode.Meanings.Select(m => m.ToBookNodeMeaningDto()).ToList(),
            Parent = bookNode.ParentBookNode.ToBookNodeThreeLevelUpperBookCoverDto(),
            Texts = bookNode.Texts.Select(n => n.ToBookTextDto()).ToList(),
        };
    }

    public static BookNodeThreeLevelUpperBookAndOneLevelLowerCoverDto ToBookNodeThreeLevelUpperBookAndOneLevelLowerDto(
        this BookNode bookNode)
    {
        if (bookNode.ParentBookNode is null)
            throw new Exception($"ParentBookNode should not be null. bookNode.Id: {bookNode.Id}");


        return new BookNodeThreeLevelUpperBookAndOneLevelLowerCoverDto
        {
            Id = bookNode.Id,
            Name = bookNode.Name,
            Description = bookNode.Description,
            Meanings = bookNode.Meanings.Select(m => m.ToBookNodeMeaningDto()).ToList(),
            Parent = bookNode.ParentBookNode.ToBookNodeTwoLevelUpperBookCoverDto(),
            Nodes = bookNode.ChildNodes.Select(n => n.ToBookNodeCoverDto()).ToList(),
        };
    }

    public static BookNodeThreeLevelUpperBookAndOneLevelLowerTextCoverDto
        ToBookNodeThreeLevelUpperBookAndOneLevelLowerTextCoverDto(
            this BookNode bookNode)
    {
        if (bookNode.ParentBookNode is null)
            throw new Exception($"ParentBookNode should not be null. bookNode.Id: {bookNode.Id}");


        return new BookNodeThreeLevelUpperBookAndOneLevelLowerTextCoverDto
        {
            Id = bookNode.Id,
            Name = bookNode.Name,
            Description = bookNode.Description,
            Meanings = bookNode.Meanings.Select(m => m.ToBookNodeMeaningDto()).ToList(),

            Parent = bookNode.ParentBookNode.ToBookNodeTwoLevelUpperBookCoverDto(),
            Texts = bookNode.Texts.Select(n => n.ToBookTextDto()).ToList(),
        };
    }

    public static BookNodeTwoLevelUpperBookCoverDto ToBookNodeTwoLevelUpperBookCoverDto(this BookNode bookNode)
    {
        if (bookNode.ParentBookNode is null)
            throw new Exception($"ParentBookNode should not be null. bookNode.Id: {bookNode.Id}");


        return new BookNodeTwoLevelUpperBookCoverDto
        {
            Description = bookNode.Description,
            Name = bookNode.Name,
            Id = bookNode.Id,
            Meanings = bookNode.Meanings.Select(m => m.ToBookNodeMeaningDto()).ToList(),
            Parent = bookNode.ParentBookNode.ToBookNodeOneLevelUpperBookCoverDto(),
        };
    }

    public static BookNodeThreeLevelUpperBookCoverDto ToBookNodeThreeLevelUpperBookCoverDto(this BookNode bookNode)
    {
        if (bookNode.ParentBookNode is null)
            throw new Exception($"ParentBookNode should not be null. bookNode.Id: {bookNode.Id}");


        return new BookNodeThreeLevelUpperBookCoverDto
        {
            Id = bookNode.Id,
            Name = bookNode.Name,
            Description = bookNode.Description,
            Meanings = bookNode.Meanings.Select(m => m.ToBookNodeMeaningDto()).ToList(),
            Parent = bookNode.ParentBookNode.ToBookNodeTwoLevelUpperBookCoverDto()
        };
    }

    public static BookNodeThreeLevelUpperBookAndOneLevelLowerCoverDto
        ToBookNodeThreeLevelUpperBookAndOneLevelLowerCoverDto(this BookNode bookNode)
    {
        if (bookNode.ParentBookNode is null)
            throw new Exception($"ParentBookNode should not be null. bookNode.Id: {bookNode.Id}");


        return new BookNodeThreeLevelUpperBookAndOneLevelLowerCoverDto
        {
            Id = bookNode.Id,
            Name = bookNode.Name,
            Description = bookNode.Description,
            Meanings = bookNode.Meanings.Select(m => m.ToBookNodeMeaningDto()).ToList(),
            Nodes = bookNode.ChildNodes.Select(n => n.ToBookNodeCoverDto()).ToList(),
            Parent = bookNode.ParentBookNode.ToBookNodeTwoLevelUpperBookCoverDto()
        };
    }


    public static BookTranslationTextDto ToBookTranslationTextDto(this BookTranslationText bookTranslationText)
    {
        if (bookTranslationText.Translation is null)
            throw new ArgumentNullException(
                $"Translation should not be null. BookTranslationTextId: {bookTranslationText.Id}");

        return new BookTranslationTextDto
        {
            Text = bookTranslationText.Text,
            Translation = bookTranslationText.Translation.ToBookTranslationDto(),
            Footnotes = bookTranslationText.Footnotes.Select(fn => fn.ToBookTranslationTextFootNoteDto()).ToList(),
        };
    }

    public static BookTranslationTextFootNoteDto ToBookTranslationTextFootNoteDto(
        this BookTranslationTextFootNote bookTranslationTextFootNote)
    {
        return new BookTranslationTextFootNoteDto
        {
            Index = bookTranslationTextFootNote.Index,
            Text = bookTranslationTextFootNote.Text,
            Indicator = bookTranslationTextFootNote.Indicator,
        };
    }

    public static BookTranslationDto ToBookTranslationDto(this BookTranslation bookTranslation)
    {
        if (bookTranslation.Language is null)
            throw new ArgumentNullException($"Language should not be null. BookTranslationId: {bookTranslation.Id}");


        return new BookTranslationDto
        {
            Id = bookTranslation.Id,
            Name = bookTranslation.Name,
            Language = bookTranslation.Language.ToLanguageDto(),
            Translators = bookTranslation.TranslationBookTranslators.Select(e => e.Translator)
                .Select(t => t.ToTranslatorDto()).ToList(),
        };
    }

    public static BookNodeFourLevelUpperBookCoverDto ToBookNodeFourLevelUpperBookCoverDto(this BookNode bookNode)
    {
        if (bookNode.ParentBookNode is null)
            throw new Exception($"ParentBookNode should not be null. bookNode.Id: {bookNode.Id}");

        return new BookNodeFourLevelUpperBookCoverDto
        {
            Description = bookNode.Description,
            Name = bookNode.Name,
            Id = bookNode.Id,
            Parent = bookNode.ParentBookNode.ToBookNodeThreeLevelUpperBookCoverDto(),
            Meanings = bookNode.Meanings.Select(m => m.ToBookNodeMeaningDto()).ToList()
        };
    }

    public static BookNodeFiveLevelUpperBookAndOneLevelLowerCoverDto
        ToBookNodeFiveLevelUpperBookAndOneLevelLowerCoverDto(this BookNode bookNode)
    {
        return new BookNodeFiveLevelUpperBookAndOneLevelLowerCoverDto
        {
            Parent = bookNode.ParentBookNode.ToBookNodeFourLevelUpperBookCoverDto(),
            Description = bookNode.Description,
            Name = bookNode.Name,
            Id = bookNode.Id,
            Meanings = bookNode.Meanings.Select(m => m.ToBookNodeMeaningDto()).ToList(),
            Nodes = bookNode.ChildNodes.Select(n => n.ToBookNodeCoverDto()).ToList()
        };
    }
    
    public static BookNodeFiveLevelUpperBookAndOneLevelLowerTextCoverDto
        ToBookNodeFiveLevelUpperBookAndOneLevelLowerTextCoverDto(this BookNode bookNode)
    {   
        if (bookNode.ParentBookNode is null)
            throw new Exception($"ParentBookNode should not be null. bookNode.Id: {bookNode.Id}");


        return new BookNodeFiveLevelUpperBookAndOneLevelLowerTextCoverDto
        {
            Parent = bookNode.ParentBookNode.ToBookNodeFourLevelUpperBookCoverDto(),
            Description = bookNode.Description,
            Name = bookNode.Name,
            Id = bookNode.Id,
            Meanings = bookNode.Meanings.Select(m => m.ToBookNodeMeaningDto()).ToList(),
            Texts = bookNode.Texts.Select(m => m.ToBookTextDto()).ToList()
        };
    }
}