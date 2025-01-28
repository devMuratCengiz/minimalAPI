using Microsoft.AspNetCore.Diagnostics;
using Scalar.AspNetCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//DI Registration

builder.Services.AddSingleton<IBookService,BookService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseExceptionHandler((appError) =>
{
    appError.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

        if (contextFeature is not null)
        {
            context.Response.StatusCode = contextFeature.Error switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                ValidationException => StatusCodes.Status422UnprocessableEntity,
                ArgumentOutOfRangeException => StatusCodes.Status400BadRequest,
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError,

            };
            await context.Response.WriteAsync(
                new ErrorDetails()
                {
                    StatusCode = context.Response.StatusCode,
                    Message = contextFeature.Error.Message
                }.ToString());
        }

    });
});

app.MapGet("/api/books", (IBookService service) =>
{
    return service.Count > 0
        ? Results.Ok(service.GetBooks())
        : Results.NoContent();

})
    .Produces<Book>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status204NoContent)
    .WithTags("GET");

app.MapGet("/api/books/{id}", (int id , IBookService service) =>
{
    var book = service.GetBookById(id);
    return book is not null ? Results.Ok(book) : Results.NotFound();
})
    .Produces<Book>(StatusCodes.Status200OK)
    .Produces<ErrorDetails>(StatusCodes.Status404NotFound)
    .WithTags("GET");

app.MapPost("/api/books", (Book newBook,IBookService service) =>
{
    var validationResults = new List<ValidationResult>();
    var context = new ValidationContext(newBook);
    bool isValid = Validator.TryValidateObject(newBook, context, validationResults, true);

    if (!isValid)
    {
        return Results.UnprocessableEntity(validationResults); //422
    }

    service.AddBook(newBook);
    return Results.Created("/api/books/" + newBook.Id, newBook);
})
    .Produces<Book>(StatusCodes.Status201Created)
    .Produces<List<ValidationResult>>(StatusCodes.Status422UnprocessableEntity)
    .WithTags("CRUD");

app.MapPut("/api/books/{id}", (int id, Book updateBook,IBookService service) =>
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
        throw new ValidationException(validationResults.First().ErrorMessage);
    }

    var book = service.UpdateBook(id, updateBook);

    return Results.Ok(book);
})
    .Produces<Book>(StatusCodes.Status200OK)
    .Produces<ErrorDetails>(StatusCodes.Status404NotFound)
    .Produces<ErrorDetails>(StatusCodes.Status400BadRequest)
    .Produces<ErrorDetails>(StatusCodes.Status422UnprocessableEntity)
    .WithTags("CRUD");

app.MapDelete("/api/books", (int id,IBookService service) =>
{
    if (!(id > 0 && id <= 1000))
    {
        throw new ArgumentOutOfRangeException("1-1000");
    }

    service.DeleteBook(id);
    return Results.NoContent();
})
    .Produces(StatusCodes.Status204NoContent)
    .Produces<ErrorDetails>(StatusCodes.Status404NotFound)
    .Produces<ErrorDetails>(StatusCodes.Status400BadRequest)
    .WithTags("CRUD");

app.MapGet("/api/books/search", (string title,IBookService service) =>
{
    var book = string.IsNullOrEmpty(title) ? service.GetBooks() : service.GetBooks().Where(b => b.Title != null && b.Title.ToLower().Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();

    return book.Any() ? Results.Ok(book) : Results.NoContent();
})
    .Produces<Book>(StatusCodes.Status200OK)
    .Produces<ErrorDetails>(StatusCodes.Status204NoContent)
    .WithTags("GET");

app.Run();

class Book
{
    [Required]
    public int Id { get; set; }
    [MinLength(2, ErrorMessage = "Min len. must be 2")]
    [MaxLength(20, ErrorMessage = "Max len. must be 20")]
    public string? Title { get; set; }
    [Range(10, 100)]
    public decimal Price { get; set; }

}

interface IBookService
{
    int Count { get; }
    List<Book> GetBooks();
    Book? GetBookById(int id);
    void AddBook(Book book);
    Book UpdateBook(int id, Book book);
    void DeleteBook(int id);
}

class BookService : IBookService
{
    private readonly List<Book> _booklist;

    public BookService()
    {
        //seed data
        _booklist = new List<Book>()
        {
            new Book{Id =1,Title ="Ýnce Memed",Price =20},
            new Book{Id =2,Title ="Kuyucaklý Yusuf",Price =15.5M},
            new Book{Id =3,Title ="Çalýkuþu",Price =18.75M},
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

public class ErrorDetails
{
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public string AtOccured => DateTime.Now.ToLongDateString();
    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}

public abstract class NotFoundException : Exception
{
    protected NotFoundException(string message) : base(message)
    {

    }
}

public sealed class BookNotFoundException : NotFoundException
{
    public BookNotFoundException(int id) : base($"The book with {id} could not be found.")
    {

    }
}

