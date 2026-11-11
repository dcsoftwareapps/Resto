namespace Resto.Domain.Platform.Abstractions;

public interface IClock
{
    DateTimeOffset UtcNow { get; }
    DateOnly BusinessDay(DateTimeOffset utc);
}
