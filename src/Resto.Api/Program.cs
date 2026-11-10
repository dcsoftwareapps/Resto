using Microsoft.EntityFrameworkCore;
using Resto.Api.Security;
using Resto.Domain.Platform.Abstractions;
using Resto.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();
builder.Services.AddScoped<IAuditLog, AuditLog>();
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
