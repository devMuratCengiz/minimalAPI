using ch_08_di.Entities.DTOs;

namespace Abstract;
public interface IBookService
{
    int Count { get; }
    List<Book> GetBooks();
    Book? GetBookById(int id);
    Book AddBook(BookDtoForInsertion book);
    Book UpdateBook(int id, BookDtoForUpdate book);
    void DeleteBook(int id);
}

