using Microsoft.AspNetCore.Diagnostics;
using Scalar.AspNetCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    //all
    options.AddPolicy("all", builder =>
    {
        builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });

    //special
    options.AddPolicy("special", builder =>
    {
        builder.WithOrigins("https://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseCors("all");

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

app.MapGet("/api/error", () =>
{
    throw new Exception("An error has been occured.");
});

app.MapGet("/api/books", () =>
{
    return Results.Ok(Book.List);
});

app.MapGet("/api/books/{id}", (int id) =>
{
    var book = Book.List.FirstOrDefault(b => b.Id == id);
    return book is not null ? Results.Ok(book) : throw new BookNotFoundException(id);
});

app.MapPost("/api/books", (Book newBook) =>
{
    newBook.Id = Book.List.Max(x => x.Id) + 1;
    Book.List.Add(newBook);
    return Results.Created("/api/books/" + newBook.Id, newBook);
});

app.MapPut("/api/books/{id}", (int id, Book updateBook) =>
{
    var book = Book.List.FirstOrDefault(x => x.Id == id);
    if (book is null)
    {
        return Results.NotFound();
    }

    book.Title = updateBook.Title;
    book.Price = updateBook.Price;

    return Results.Ok(book);
});

app.MapDelete("/api/books", (int id) =>
{
    var book = Book.List.FirstOrDefault(x => x.Id == id);
    if (book is null)
    {
        return Results.NotFound();
    }

    Book.List.Remove(book);
    return Results.NoContent();
});

app.MapGet("/api/books/search", (string title) =>
{
    var book = string.IsNullOrEmpty(title) ? Book.List : Book.List.Where(b => b.Title != null && b.Title.ToLower().Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();

    return book.Any() ? Results.Ok(book) : Results.NoContent();
});

app.Run();

class Book
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public decimal Price { get; set; }

    //seed data

    private static List<Book> _bookList = new List<Book>()
    {
        new Book{Id =1,Title ="Ýnce Memed",Price =20},
        new Book{Id =2,Title ="Kuyucaklý Yusuf",Price =15.5M},
        new Book{Id =3,Title ="Çalýkuþu",Price =18.75M},
    };

    public static List<Book> List => _bookList;
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

