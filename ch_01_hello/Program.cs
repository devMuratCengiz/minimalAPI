using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

var message = "Hello Worldd";

app.MapGet("/hello", () =>
{
    return new Response(message);
});

app.UseHttpsRedirection();

app.Run();

class Response
{
    public Response(string msg)
    {
        Message = msg;
    }
    public string? Message { get; set; }
    public DateTime? Date => DateTime.Now;
}
