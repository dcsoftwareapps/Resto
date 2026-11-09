using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Resto.Domain.Platform;
using Resto.Domain.Platform.Abstractions;
using Resto.Domain.Users;

namespace Resto.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    private readonly ICurrentUser? _currentUser;

    // A missing identity permits design-time model generation only. Data operations fail closed.
    public AppDbContext(DbContextOptions options, ICurrentUser? currentUser) : base(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    internal ICurrentUser RequireCurrentUser()
    {
        if (_currentUser is null || _currentUser.UserId == Guid.Empty ||
            _currentUser.OrganizationId == Guid.Empty || _currentUser.RestaurantId == Guid.Empty)
        {
            throw new InvalidOperationException("A valid authorized user, organization and restaurant context is required.");
        }

        return _currentUser;
    }

    private Guid OrganizationId => RequireCurrentUser().OrganizationId;
    private Guid RestaurantId => RequireCurrentUser().RestaurantId;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new EntityMetadataInterceptor());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var organizations = modelBuilder.Entity<Organization>();
        organizations.ToTable("organizations");
        organizations.HasQueryFilter(x => x.Id == OrganizationId &&
            Restaurants.Any(r => r.Id == RestaurantId && r.OrganizationId == OrganizationId));
        organizations.Property(x => x.Name).IsRequired();
        organizations.Property(x => x.LegalName).IsRequired();

        var restaurants = modelBuilder.Entity<Restaurant>();
        restaurants.ToTable("restaurants");
        restaurants.HasQueryFilter(x => x.Id == RestaurantId && x.OrganizationId == OrganizationId);
        restaurants.HasOne<Organization>().WithMany().HasForeignKey(x => x.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
        restaurants.HasAlternateKey(x => new { x.Id, x.OrganizationId });
        restaurants.Property(x => x.OrganizationId).IsConcurrencyToken();
        restaurants.Property(x => x.TimeZone).HasDefaultValue("America/Tijuana");
        restaurants.Property(x => x.BusinessDayCutoff).HasDefaultValue(new TimeOnly(4, 0));
        restaurants.Property(x => x.NextOrderNumber).HasDefaultValue(1);

        var roles = modelBuilder.Entity<Role>();
        roles.ToTable("roles");
        roles.HasAlternateKey(x => new { x.Id, x.OrganizationId, x.RestaurantId });

        var users = modelBuilder.Entity<User>();
        users.ToTable("users");
        users.HasOne<Role>().WithMany()
            .HasForeignKey(x => new { x.RoleId, x.OrganizationId, x.RestaurantId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId, x.RestaurantId })
            .OnDelete(DeleteBehavior.Restrict);

        var permissions = modelBuilder.Entity<RolePermission>();
        permissions.ToTable("role_permissions");
        permissions.HasIndex(x => new { x.RoleId, x.Permission }).IsUnique();
        permissions.HasOne<Role>().WithMany()
            .HasForeignKey(x => new { x.RoleId, x.OrganizationId, x.RestaurantId })
            .HasPrincipalKey(x => new { x.Id, x.OrganizationId, x.RestaurantId })
            .OnDelete(DeleteBehavior.Restrict);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes().ToList())
        {
            if (!typeof(Entity).IsAssignableFrom(entityType.ClrType))
            {
                continue;
            }

            var entity = modelBuilder.Entity(entityType.ClrType);
            entity.HasKey(nameof(Entity.Id));
            entity.Property(nameof(Entity.Id)).ValueGeneratedNever();
            entity.Property(nameof(Entity.IsActive)).HasDefaultValue(true);

            if (typeof(TenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "entity");
                var scoped = Expression.AndAlso(
                    Expression.Equal(Expression.Property(parameter, nameof(TenantEntity.OrganizationId)),
                        Expression.Property(Expression.Constant(this, typeof(AppDbContext)), nameof(OrganizationId))),
                    Expression.Equal(Expression.Property(parameter, nameof(TenantEntity.RestaurantId)),
                        Expression.Property(Expression.Constant(this, typeof(AppDbContext)), nameof(RestaurantId))));
                entity.HasQueryFilter(Expression.Lambda(scoped, parameter));
                entity.Property(nameof(TenantEntity.OrganizationId)).IsConcurrencyToken();
                entity.Property(nameof(TenantEntity.RestaurantId)).IsConcurrencyToken();
                entity.HasOne(typeof(Restaurant)).WithMany()
                    .HasForeignKey(nameof(TenantEntity.RestaurantId), nameof(TenantEntity.OrganizationId))
                    .HasPrincipalKey(nameof(Entity.Id), nameof(Restaurant.OrganizationId))
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}
