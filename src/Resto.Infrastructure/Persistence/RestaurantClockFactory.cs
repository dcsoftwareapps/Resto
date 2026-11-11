using Microsoft.EntityFrameworkCore;
using Resto.Domain.Platform;
using Resto.Domain.Platform.Abstractions;

namespace Resto.Infrastructure.Persistence;

public sealed class RestaurantClockFactory(AppDbContext context, TimeProvider timeProvider)
{
    public async Task<IClock> CreateAsync(CancellationToken cancellationToken = default)
    {
        var settings = await context.Restaurants.AsNoTracking()
            .Select(restaurant => new { restaurant.TimeZone, restaurant.BusinessDayCutoff })
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new InvalidOperationException("The authorized restaurant has no clock settings.");

        return new BusinessClock(timeProvider, TimeZoneInfo.FindSystemTimeZoneById(settings.TimeZone),
            settings.BusinessDayCutoff);
    }
}
