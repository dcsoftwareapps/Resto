using Microsoft.EntityFrameworkCore;
using Resto.Api.Development;
using Resto.Api.Security;
using Resto.Domain.Platform.Abstractions;
using Resto.Infrastructure.Persistence;

var seedRequested = DevSeed.IsRequested(args);
var builder = WebApplication.CreateBuilder(seedRequested ? [] : args);
if (seedRequested)
{
    var created = await DevSeed.RunAsync(builder.Environment.EnvironmentName,
        builder.Configuration.GetConnectionString("Resto"));
    Console.WriteLine(created ? "Datos del demo creados." : "Los datos del demo ya existen.");
    return;
}

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();
builder.Services.AddScoped<IAuditLog, AuditLog>();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<RestaurantClockFactory>();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Resto");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Set ConnectionStrings__Resto to use Resto persistence.");
    }

    options.UseNpgsql(connectionString);
});
var app = builder.Build();
app.Run();

public partial class Program;
