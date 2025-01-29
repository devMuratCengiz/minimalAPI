namespace Abstract;
public interface IBookService
{
    int Count { get; }
    List<Book> GetBooks();
    Book? GetBookById(int id);
    void AddBook(Book book);
    Book UpdateBook(int id, Book book);
    void DeleteBook(int id);
}

