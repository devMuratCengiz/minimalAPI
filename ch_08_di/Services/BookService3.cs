using Abstract;
using AutoMapper;
using ch_08_di.Configuration;
using ch_08_di.Entities.DTOs.Book;
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
            Validate<BookDtoForInsertion>(book);

            var newBook = _mapper.Map<Book>(book);
            _repository.Add(newBook);
            return newBook;
        }

        public void DeleteBook(int id)
        {
            id.ValidateIdInRange();
            var book = _repository.Get(id);
            if (book != null)
            {
                _repository.Delete(book);
            }
            else
            {
                throw new BookNotFoundException(id);
            }
            
        }

        public BookDto? GetBookById(int id)
        {
            id.ValidateIdInRange();
            var book = _repository.Get(id);
            if (book is not null)
            {
                return _mapper.Map<BookDto>(book);
            }
            
            throw new BookNotFoundException(id);
        }

        public ICollection<BookDto> GetBooks()
        {
            var books = _repository.GetAll();
            return _mapper.Map<List<BookDto>>(books);
        }

        public Book UpdateBook(int id, BookDtoForUpdate updateBook)
        {

            id.ValidateIdInRange();

            Validate<BookDtoForUpdate>(updateBook);

            var book = _repository.Get(id);

            book = _mapper.Map(updateBook,book);
            _repository.Update(book);
            
            return book;
        }

        private void Validate<T>(T item)
        {
            var validationResult = new List<ValidationResult>();
            var context = new ValidationContext(item);
            var isValid = Validator.TryValidateObject(item, context, validationResult, true);

            if (!isValid)
            {
                var errors = string.Join(" ", validationResult.Select(v => v.ErrorMessage));
                throw new ValidationException(errors);
            }
        }
    }
}
