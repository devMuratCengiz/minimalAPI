using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}
String Hello()
{
    return "[Local Function] Hello World.";
}
var variableLambda = () => "[Variable] Hello World.";

app.MapGet("/hello", () => "Hello World.");           //inline
app.MapPost("/hello", variableLambda);                //lambda variable
app.MapPut("/hello", Hello);                          //local function
app.MapDelete("/hello",new HelloHandler().Hello);     //instance member

app.UseHttpsRedirection();

app.Run();


class HelloHandler
{
    public String? Hello()
    {
        return "[Instance Member] Hello World.";
    }
}


