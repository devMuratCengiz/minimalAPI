using Abstract;
using ch_08_di.Repositories;

namespace ch_08_di.Services
{
    public class BookService3 : IBookService
    {
        private readonly BookRepository _repository;

        public BookService3(BookRepository repository)
        {
            _repository = repository;
        }

        public int Count => _repository.GetAll().Count;

        public void AddBook(Book book)
        {
            _repository.Add(book);
        }

        public void DeleteBook(int id)
        {
            var book = _repository.Get(id);
            if (book != null)
            {
                _repository.Remove(book);
            }
            else
            {
                throw new BookNotFoundException(id);
            }
            
        }

        public Book? GetBookById(int id)
        {
            return _repository.Get(id);
        }

        public List<Book> GetBooks()
        {
            return _repository.GetAll();
        }

        public Book UpdateBook(int id, Book updateBook)
        {
            var book = _repository.Get(id);
            if (book != null)
            {
                book.Title = updateBook.Title;
                book.Price = updateBook.Price;
                _repository.Update(book);
            }
            else
            {
                throw new BookNotFoundException(id);
            }
            return book;
        }
    }
}
