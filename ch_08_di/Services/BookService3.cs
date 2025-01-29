using Abstract;
using AutoMapper;
using ch_08_di.Entities.DTOs;
using ch_08_di.Repositories;
using System.ComponentModel.DataAnnotations;

namespace ch_08_di.Services
{
    public class BookService3 : IBookService
    {
        private readonly BookRepository _repository;
        private readonly IMapper _mapper;

        public BookService3(BookRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public int Count => _repository.GetAll().Count;

        public Book AddBook(BookDtoForInsertion book)
        {
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(book);
            bool isValid = Validator.TryValidateObject(book, context, validationResults, true);

            if (!isValid)
            {
                var errors = string.Join(" ", validationResults.Select(v => v.ErrorMessage));
                throw new ValidationException(errors);
            }

            var newBook = _mapper.Map<Book>(book);
            _repository.Add(newBook);
            return newBook;
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
            var book = _repository.Get(id);
            if (book is not null)
            {
                return book;
            }
            
            throw new BookNotFoundException(id);
        }

        public List<Book> GetBooks()
        {
            return _repository.GetAll();
        }

        public Book UpdateBook(int id, BookDtoForUpdate updateBook)
        {

            if (!(id > 0 && id <= 1000))
            {
                throw new ArgumentOutOfRangeException("1-1000");
            }

            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(updateBook);
            var isValid = Validator.TryValidateObject(updateBook, context, validationResults, true);

            if (!isValid)
            {
                var errors = string.Join(" ", validationResults.Select(v => v.ErrorMessage));
                throw new ValidationException(errors);
            }

            var book = _repository.Get(id);

            book = _mapper.Map(updateBook,book);
            _repository.Update(book);
            
            return book;
        }
    }
}
