using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/employees", () =>
{
    return new Employee().GetAllEmployees();
});

app.MapGet("/employees/{id}" , (int id) =>
{
    return new Employee().GetOneEmployee(id);
});

app.MapGet("/employees/search", (string q) => Employee.Search(q));

app.MapPost("/employees", (Employee emp) =>
{
    Employee.CreateEmployee(emp);
});

app.Run();


class Employee
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public decimal Salary { get; set; }
    private static List<Employee> Employees = new()
    {
        new Employee() { Id = 1, FullName = "Ahmet Güneþ", Salary = 90_000 },
        new Employee() { Id = 2, FullName = "Sýla Bulut", Salary = 70_000 },
        new Employee() { Id = 3, FullName = "Can Tan", Salary = 95_000 }
    };

    public List<Employee> GetAllEmployees() => Employees;
    public Employee? GetOneEmployee(int id) => Employees.SingleOrDefault(e => e.Id == id);

    public static void CreateEmployee(Employee employee)
    {
        Employees.Add(employee);
    }

    public static List<Employee>? Search(string q)
    {
        return Employees.Where(e => e.FullName != null && e.FullName.ToLower().Contains(q.ToLower())).ToList();
    }
}