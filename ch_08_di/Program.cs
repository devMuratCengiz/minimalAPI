using Abstract;
using ch_08_di.Repositories;
using ch_08_di.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Services;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//DI Registration

builder.Services.AddScoped<BookRepository>();

builder.Services.AddScoped<IBookService,BookService3>();

builder.Services.AddDbContext<RepositoryContext>(options=>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

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

public class Book
{
    [Required]
    public int Id { get; set; }
    [MinLength(2, ErrorMessage = "Min len. must be 2")]
    [MaxLength(20, ErrorMessage = "Max len. must be 20")]
    public string? Title { get; set; }
    [Range(10, 100)]
    public decimal Price { get; set; }

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

