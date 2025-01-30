using ch_08_di.Entities.DTOs.Book;

namespace Abstract;
public interface IBookService
{
    int Count { get; }
    ICollection<BookDto> GetBooks();
    BookDto? GetBookById(int id);
    Book AddBook(BookDtoForInsertion book);
    Book UpdateBook(int id, BookDtoForUpdate book);
    void DeleteBook(int id);
}

