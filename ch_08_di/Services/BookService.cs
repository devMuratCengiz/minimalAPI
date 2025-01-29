using Abstract;

namespace Services;

public class BookService : IBookService
{
    private readonly List<Book> _booklist;

    public BookService()
    {
        //seed data
        _booklist = new List<Book>()
        {
            new Book{Id =1,Title ="İnce Memed",Price =20},
            new Book{Id =2,Title ="Kuyucaklı Yusuf",Price =15.5M},
            new Book{Id =3,Title ="Çalıkuşu",Price =18.75M}
        };

    }

    public List<Book> GetBooks () => _booklist;

    public int Count => _booklist.Count;

    public Book? GetBookById(int id) =>
    _booklist.FirstOrDefault(b => b.Id == id);

    public void AddBook(Book newBook)
    {
        newBook.Id = _booklist.Max(b => b.Id) + 1;
        _booklist.Add(newBook);
    }

    public Book UpdateBook(int id,Book updateBook)
    {
        var book = _booklist.FirstOrDefault(b => b.Id == id);
        if (book == null)
        {
            throw new BookNotFoundException(id);
        }
            book.Title = updateBook.Title;
            book.Price = updateBook.Price;

        return book;
    }

    public void DeleteBook(int id)
    {
        var book = _booklist.FirstOrDefault(b => b.Id == id);
        if(book is not null)
        {
            _booklist.Remove(book);
        }
        else
        {
            throw new BookNotFoundException(id);
        }
    }
    }

