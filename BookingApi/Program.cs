using Booking.Application;
using Booking.Application.Users.Register;
using Booking.Domain.Entities;
using Booking.Domain.Users;
using Booking.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddOpenApi();

builder.Services.ConfigureInfrastructureServices(builder.Configuration);
builder.Services.ConfigureApplicationServices();


var app = builder.Build();


/*
 * shtimi manual i nje useri ne db
 

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BookingDbContext>();

    //if (!context.Users.Any(u => u.Email == "aldin.xhaxhamani@gmail.com"))
    //{
        var hash = "password1";

        var user = new User(
            email: "aldin.xhaxhamani@gmail.com",
            name: "Aldin",
            lastName: "Xhaxhamani",
            password: hash,
            phoneNumber: "+123",
            profileImageUrl: "UrlEFotos"
        );

        context.Users.Add(user);
        context.SaveChanges();
    //}
}

*/


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
