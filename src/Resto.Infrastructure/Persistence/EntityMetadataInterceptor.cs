using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Resto.Domain.Platform;

namespace Resto.Infrastructure.Persistence;

public sealed class EntityMetadataInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var context = (AppDbContext)eventData.Context!;
        Prepare(context);
        if (!HasNewRestaurant(context) && !context.Restaurants.AsNoTracking().Any())
        {
            throw new InvalidOperationException("The restaurant does not belong to the authorized organization context.");
        }
        return result;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var context = (AppDbContext)eventData.Context!;
        Prepare(context);
        if (!HasNewRestaurant(context) && !await context.Restaurants.AsNoTracking().AnyAsync(cancellationToken))
        {
            throw new InvalidOperationException("The restaurant does not belong to the authorized organization context.");
        }
        return result;
    }

    private static bool HasNewRestaurant(AppDbContext context) =>
        context.ChangeTracker.Entries<Restaurant>().Any(x => x.State == EntityState.Added);

    private static void Prepare(AppDbContext context)
    {
        var user = context.RequireCurrentUser();
        var now = DateTimeOffset.UtcNow;
        // PostgreSQL timestamps have microsecond precision.
        now = new DateTimeOffset(now.Ticks - now.Ticks % 10, TimeSpan.Zero);
        foreach (var entry in context.ChangeTracker.Entries<Entity>())
        {
            if (entry.State == EntityState.Deleted)
            {
                throw new InvalidOperationException("Business entities must be deactivated instead of deleted.");
            }
            if (entry.State is not (EntityState.Added or EntityState.Modified))
            {
                continue;
            }

            switch (entry.Entity)
            {
                case Organization organization when organization.Id != user.OrganizationId:
                    throw new InvalidOperationException("Organization is outside the authorized scope.");
                case Restaurant restaurant:
                    if (restaurant.OrganizationId == Guid.Empty && entry.State == EntityState.Added)
                    {
                        restaurant.OrganizationId = user.OrganizationId;
                    }
                    if (restaurant.Id != user.RestaurantId || restaurant.OrganizationId != user.OrganizationId)
                    {
                        throw new InvalidOperationException("Restaurant is outside the authorized scope.");
                    }
                    break;
                case TenantEntity tenant:
                    if (entry.State == EntityState.Added)
                    {
                        if (tenant.OrganizationId == Guid.Empty)
                        {
                            tenant.OrganizationId = user.OrganizationId;
                        }
                        if (tenant.RestaurantId == Guid.Empty)
                        {
                            tenant.RestaurantId = user.RestaurantId;
                        }
                    }
                    if (tenant.OrganizationId != user.OrganizationId || tenant.RestaurantId != user.RestaurantId)
                    {
                        throw new InvalidOperationException("Entity is outside the authorized scope.");
                    }
                    break;
            }

            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.CreatedBy = user.UserId;
                entry.Entity.UpdatedAt = null;
                entry.Entity.UpdatedBy = null;
            }
            else
            {
                entry.Property(x => x.CreatedAt).IsModified = false;
                entry.Property(x => x.CreatedBy).IsModified = false;
                entry.Entity.UpdatedAt = now;
                entry.Entity.UpdatedBy = user.UserId;
                foreach (var property in entry.Properties.Where(x => x.Metadata.IsConcurrencyToken))
                {
                    if (!Equals(property.OriginalValue, property.CurrentValue))
                    {
                        throw new InvalidOperationException("Tenant ownership cannot be changed.");
                    }
                }
            }
        }
    }
}
