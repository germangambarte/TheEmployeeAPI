using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TheEmployeeAPI;
using TheEmployeeAPI.Abstractions;
using TheEmployeeAPI.Employees;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IRepository<Employee>, EmployeeRepository>();
builder.Services.AddProblemDetails();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

var employeeRoute = app.MapGroup("employees");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

employeeRoute.MapGet(string.Empty, (IRepository<Employee> repository) =>
{
    var employees = repository.GetAll();
    return Results.Ok(employees.Select(employee => new GetEmployeeResponse
    {
        FirstName = employee.FirstName,
        LastName = employee.LastName,
        Address1 = employee.Address1,
        Address2 = employee.Address2,
        City = employee.City,
        State = employee.State,
        ZipCode = employee.ZipCode,
        PhoneNumber = employee.PhoneNumber,
        Email = employee.Email,
    }));
});

employeeRoute.MapGet("/{id:int}", (int id, IRepository<Employee> repository) =>
{
    var employee = repository.GetById(id);

    if (employee == null) return Results.NotFound();

    return Results.Ok(new GetEmployeeResponse
    {
        FirstName = employee.FirstName,
        LastName = employee.LastName,
        Address1 = employee.Address1,
        Address2 = employee.Address2,
        City = employee.City,
        State = employee.State,
        ZipCode = employee.ZipCode,
        PhoneNumber = employee.PhoneNumber,
        Email = employee.Email,
    });
});

employeeRoute.MapPost(string.Empty,
    async ([FromBody] CreateEmployeeRequest employeeRequest, IRepository<Employee> repository,
    IValidator<CreateEmployeeRequest> validator
    ) =>
    {
        var validatorResults = await validator.ValidateAsync(employeeRequest);
        if (!validatorResults.IsValid)
        {
            return Results.ValidationProblem(validatorResults.ToDictionary());
        }

        var newEmployee = new Employee
        {
            FirstName = employeeRequest.FirstName!,
            LastName = employeeRequest.LastName!,
            SocialSecurityNumber = employeeRequest.SocialSecurityNumber,
            Address1 = employeeRequest.Address1,
            Address2 = employeeRequest.Address2,
            City = employeeRequest.City,
            State = employeeRequest.State,
            ZipCode = employeeRequest.ZipCode,
            PhoneNumber = employeeRequest.PhoneNumber,
            Email = employeeRequest.Email
        };
        repository.Create(newEmployee);
        return Results.Created($"/employees/{newEmployee.Id}", employeeRequest);
    });

employeeRoute.MapPut("{id:int}",
    ([FromBody] UpdateEmployeeRequest employeeRequest, int id, IRepository<Employee> repository) =>
    {
        var existingEmployee = repository.GetById(id);

        if (existingEmployee == null) return Results.NotFound();

        existingEmployee.Address1 = employeeRequest.Address1;
        existingEmployee.Address2 = employeeRequest.Address2;
        existingEmployee.City = employeeRequest.City;
        existingEmployee.State = employeeRequest.State;
        existingEmployee.ZipCode = employeeRequest.ZipCode;
        existingEmployee.PhoneNumber = employeeRequest.PhoneNumber;
        existingEmployee.Email = employeeRequest.Email;

        repository.Update(existingEmployee);

        return Results.Ok(existingEmployee);
    });

app.Run();

public partial class Program
{
}