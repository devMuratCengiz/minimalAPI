using Abstract;
using ch_08_di.Configuration;
using ch_08_di.Entities.DTOs;
using ch_08_di.Repositories;
using ch_08_di.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//DI Registration

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

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

app.UseCustomExceptionHandler();

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
    return Results.Ok(book);
})
    .Produces<Book>(StatusCodes.Status200OK)
    .Produces<ErrorDetails>(StatusCodes.Status404NotFound)
    .WithTags("GET");

app.MapPost("/api/books", (BookDtoForInsertion newBook,IBookService service) =>
{
    var book = service.AddBook(newBook);
    return Results.Created("/api/books/" + book.Id, newBook);
})
    .Produces<Book>(StatusCodes.Status201Created)
    .Produces<List<ValidationResult>>(StatusCodes.Status422UnprocessableEntity)
    .WithTags("CRUD");

app.MapPut("/api/books/{id}", (int id, BookDtoForUpdate updateBook,IBookService service) =>
{

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

