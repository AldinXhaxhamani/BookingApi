using Booking.Application;
using Booking.Application.Apartaments.Register;
using Booking.Application.Users.Register;
using Booking.Domain.Apartments;
using Booking.Domain.Entities;
using Booking.Domain.Users;
using Booking.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);



builder.Services.AddOpenApi();

builder.Services.ConfigureInfrastructureServices(builder.Configuration);
builder.Services.ConfigureApplicationServices();


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");





//Krijimi i nje API Register User 

app.MapPost("/api/user/register",
    async (CreateUserDto dto, [FromServices] IMediator mediator, CancellationToken ct) =>
    {

        var command = new RegisterUserCommand
        {
            createUserDto = dto
        };

        var id = await mediator.Send(command, ct);
        //return Results.Created($"/api/users/{id}", new { id });
        return Results.Ok(id);
    }).RequireAuthorization(policy => policy.RequireRole("User"));



app.MapPost("/api/property/register",   
    async (CreatePropertyDto dto, [FromServices] IMediator mediator, CancellationToken ct) =>
    {
        
        var command = new RegisterPropertyCommand
        {
            createPropertyDto = dto
        };
  
        using var scope = app.Services.CreateScope();
        var handler = scope.ServiceProvider.GetService<IRequestHandler<RegisterPropertyCommand, Guid>>();
        Console.WriteLine(handler is null ? "Handler NOT registered" : "Handler registered");

        var id = await mediator.Send(command, ct);
        //return Results.Created($"/api/users/{id}", new { id });
        return Results.Ok(id);
    });


//Testimi i lidhjes me db
app.MapGet("/dbtest", async (BookingDbContext db) =>
{
    var count = await db.Users.CountAsync();
    return Results.Ok(new { users = count });
});


app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
