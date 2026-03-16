using Booking.Application;
using Booking.Application.Apartaments.Availability;
using Booking.Application.Apartaments.Discount;
using Booking.Application.Apartaments.NewFolder;
using Booking.Application.Apartaments.Register;
using Booking.Application.Apartaments.Search;
using Booking.Application.Apartaments.Search.Get;
using Booking.Application.Apartaments.Stay_Duration;
using Booking.Application.Apartaments.Update;
using Booking.Application.Login;
using Booking.Application.Logout;
using Booking.Application.Roles.Assign;
using Booking.Application.Users.ChangePassword;
using Booking.Application.Users.Delete;
using Booking.Application.Users.Photo;
using Booking.Application.Users.Register;
using Booking.Application.Users.Update;
using Booking.Domain.Apartments.DTOs;
using Booking.Domain.Entities;
using Booking.Domain.Users;
using Booking.Infrastructure;
using BookingApi.Binders;
using BookingApi.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;



var builder = WebApplication.CreateBuilder(args);



builder.Services.AddOpenApi();

builder.Services.ConfigureInfrastructureServices(builder.Configuration);
builder.Services.ConfigureApplicationServices();


builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();


app.UseStaticFiles();
app.UseExceptionHandler();
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
        return Results.Created($"/api/users/{id}", new { id });
       
    });



app.MapPost("/api/property/register",
    async (
    CreatePropertyDto dto,
    HttpContext context,
    IMediator mediator,
    CancellationToken ct) =>
    {
        var ownerId = Guid.Parse(context.User.FindFirstValue("sub")!); // ← from JWT

        var command = new RegisterPropertyCommand
        {
            OwnerId = ownerId,
            CreatePropertyDto = dto
        };

        var propertyId = await mediator.Send(command, ct);
        return Results.Ok(new { propertyId });

    }).RequireAuthorization(p => p.RequireRole("Owner"));


//Testimi i lidhjes me db
app.MapGet("/dbtest", async (BookingDbContext db) =>
{
    var count = await db.Users.CountAsync();
    return Results.Ok(new { users = count });
});

app.MapPost("/auth/login", async (
    LoginCommand command,
    IMediator mediator,
    CancellationToken ct) =>
    {
    var response = await mediator.Send(command, ct);
    return Results.Ok(response);
    });


app.MapPost("/auth/logout", async (
    HttpContext context,
    IMediator mediator,
    CancellationToken ct) =>
{
    var accessToken = context.Request.Headers["Authorization"]
        .ToString().Replace("Bearer ", "");

    var userId = Guid.Parse(
        context.User.FindFirstValue("sub")!);

    var command = new LogoutCommand
    {
        AccessToken = accessToken,
        UserId = userId
    };

    await mediator.Send(command, ct);

    return Results.Ok(new { message = "Logged out successfully." });
})
.RequireAuthorization();


app.MapPut("/api/user/update", async (
    UpdateUserDto dto,
    HttpContext context,
    IMediator mediator,
    CancellationToken ct) =>
{
    var command = new UpdateUserCommand
    {

        UserId = Guid.Parse(context.User.FindFirstValue("sub")!),
        updateUserDto = dto
    };

    var updatedProfile = await mediator.Send(command, ct);

    return Results.Ok(updatedProfile);
})
.RequireAuthorization();



app.MapPost("/api/user/uploadPhoto", async (
    HttpContext context,
    IMediator mediator,
    CancellationToken ct) =>
{
    //merret file nga postman(ne rastin e testimit me postman)
    var file = context.Request.Form.Files.FirstOrDefault();

    if (file is null)
        throw new InvalidOperationException("No file was uploaded.");

    
    var userId = Guid.Parse(context.User.FindFirstValue("sub")!);

    var command = new UploadProfilePhotoCommand
    {
        UserId = userId,
        FileStream = file.OpenReadStream(),
        FileName = file.FileName,
        ContentType = file.ContentType
    };

    var photoUrl = await mediator.Send(command, ct);

    return Results.Ok(new { photoUrl });
})
.RequireAuthorization();


app.MapPut("/api/user/changePassword", async (
    ChangePasswordCommand command,
    HttpContext context,
    IMediator mediator,
    CancellationToken ct) =>
{

    command.UserId = Guid.Parse(context.User.FindFirstValue("sub")!);

    await mediator.Send(command, ct);

    return Results.Ok(new { message = "Password changed successfully." });
})
.RequireAuthorization();



app.MapPost("/api/admin/assignRrole", async (
    AssignRoleCommand command,
    IMediator mediator,
    CancellationToken ct) =>
{
    await mediator.Send(command, ct);

    return Results.Ok(new { message = $"Role '{command.RoleName}' assigned successfully." });
})
.RequireAuthorization(p => p.RequireRole("Admin"));


app.MapDelete("/api/admin/deleteUser", async (
    HttpContext context,
    IMediator mediator,
    CancellationToken ct) =>
{

    var email = context.Request.Query["email"].ToString();

    if (string.IsNullOrEmpty(email))
        throw new InvalidOperationException("Email is required.");

    // marrim AdminId nga JWT
    var adminId = Guid.Parse(context.User.FindFirstValue("sub")!);

    var command = new DeleteUserCommand
    {
        TargetEmail = email,
        AdminId = adminId
    };

    await mediator.Send(command, ct);

    return Results.Ok(new { message = $"User '{email}' deactivated successfully." });
})
.RequireAuthorization(p => p.RequireRole("Admin"));


app.MapPut("/api/property/update/{propertyId}", async (
    Guid propertyId,
    CreatePropertyDto dto,      
    HttpContext context,
    IMediator mediator,
    CancellationToken ct) =>
{
    var command = new UpdatePropertyCommand
    {
        PropertyId = propertyId,
        OwnerId = Guid.Parse(context.User.FindFirstValue("sub")!),
        Dto = dto
    };

    await mediator.Send(command, ct);
    return Results.Ok(new { message = "Property updated successfully." });
})
.RequireAuthorization(p => p.RequireRole("Owner"));



app.MapGet("/api/property/{propertyId}/availability", async (
    Guid propertyId, IMediator mediator, CancellationToken ct) =>
{
    var result = await mediator.Send(
        new GetPropertyAvailabilityQuery { PropertyId = propertyId }, ct);
    return Results.Ok(result);
});




// Set season price — Owner only 
app.MapPost("/api/property/{propertyId}/season-price", async (
    Guid propertyId, SetSeasonPriceCommand command,
    HttpContext context, IMediator mediator, CancellationToken ct) =>
{
    command.PropertyId = propertyId;
    command.OwnerId = Guid.Parse(context.User.FindFirstValue("sub")!);
    await mediator.Send(command, ct);
    return Results.Ok(new { message = "Season price set successfully." });
})
.RequireAuthorization(p => p.RequireRole("Owner"));

//Set discount — Owner only 
app.MapPost("/api/property/{propertyId}/discount", async (
    Guid propertyId, SetDiscountCommand command,
    HttpContext context, IMediator mediator, CancellationToken ct) =>
{
    command.PropertyId = propertyId;
    command.OwnerId = Guid.Parse(context.User.FindFirstValue("sub")!);
    await mediator.Send(command, ct);
    return Results.Ok(new { message = "Discount set successfully." });
})
.RequireAuthorization(p => p.RequireRole("Owner"));

// Set minimum stay — Owner only 
app.MapPut("/api/property/{propertyId}/minimum-stay", async (
    Guid propertyId, SetMinimumStayCommand command,
    HttpContext context, IMediator mediator, CancellationToken ct) =>
{
    command.PropertyId = propertyId;
    command.OwnerId = Guid.Parse(context.User.FindFirstValue("sub")!);
    await mediator.Send(command, ct);
    return Results.Ok(new { message = "Minimum stay updated." });
})
.RequireAuthorization(p => p.RequireRole("Owner"));

//Set maximum stay — Owner only 
app.MapPut("/api/property/{propertyId}/maximum-stay", async (
    Guid propertyId, SetMaximumStayCommand command,
    HttpContext context, IMediator mediator, CancellationToken ct) =>
{
    command.PropertyId = propertyId;
    command.OwnerId = Guid.Parse(context.User.FindFirstValue("sub")!);
    await mediator.Send(command, ct);
    return Results.Ok(new { message = "Maximum stay updated." });
})
.RequireAuthorization(p => p.RequireRole("Owner"));



app.MapGet("/api/properties/search", async (
    HttpContext context,
    IMediator mediator,
    CancellationToken ct) =>
{
    var query = SearchPropertiesQueryBinder.BindFromRequest(context);
    var result = await mediator.Send(query, ct);
    return Results.Ok(result);
});

// Property details
app.MapGet("/api/property/{propertyId}/details", async (
    Guid propertyId,
    IMediator mediator,
    CancellationToken ct) =>
{
    var result = await mediator.Send(
        new GetPropertyDetailsQuery { PropertyId = propertyId }, ct);
    return Results.Ok(result);
});



app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
