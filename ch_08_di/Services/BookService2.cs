using Abstract;
using ch_08_di.Repositories;

namespace Services;

public class BookService2 : IBookService
{
    private readonly RepositoryContext _context;

    public BookService2(RepositoryContext repositoryContext)
    {
        _context = repositoryContext;
    }

    public int Count => _context.Books.ToList().Count;

    public void AddBook(Book book)
    {
        
        _context.Books.Add(book);
        _context.SaveChanges();
    }

    public void DeleteBook(int id)
    {
        var book = _context.Books.FirstOrDefault(x => x.Id == id);
        if(book is not null)
        {
            _context.Books.Remove(book);
            _context.SaveChanges();
        }
        else
        {
            throw new BookNotFoundException(id);
        }
    }

    public Book? GetBookById(int id)
    {
        var book = _context.Books.FirstOrDefault(x => x.Id == id);
        return book;
    }

    public List<Book> GetBooks()
    {
        var books = _context.Books.ToList();
        return books;
    }

    public Book UpdateBook(int id, Book updateBook)
    {
        var book = _context.Books.FirstOrDefault(x => x.Id == id);
        if(book is null)
        {
            throw new BookNotFoundException(id);
        }
        book.Title = updateBook.Title;
        book.Price = updateBook.Price;
        _context.SaveChanges();
        return book;
    }
}

