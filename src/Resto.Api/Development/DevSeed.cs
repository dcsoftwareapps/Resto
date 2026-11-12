using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Resto.Contracts;
using Resto.Domain.Platform;
using Resto.Domain.Platform.Abstractions;
using Resto.Domain.Users;
using Resto.Infrastructure.Persistence;

namespace Resto.Api.Development;

internal static class DevSeed
{
    private static readonly Guid OrganizationId = SeedId(1);
    private static readonly Guid RestaurantId = SeedId(2);
    internal static Guid ActorId { get; } = SeedId(3);
    private const long SeedLock = 807008;

    // Permissions follow the E07 matrix, in the same role order.
    private static readonly (string Name, string[] Grants)[] Roles =
    [
        ("Administrador",
        [
            Permissions.PlatformRemoteLogin,
            Permissions.PlatformDevices,
            Permissions.OrdersView,
            Permissions.PosTake,
            Permissions.PosCancel,
            Permissions.PosDiscount,
            Permissions.PosReprint,
            Permissions.PosTransfer,
            Permissions.CashView,
            Permissions.CashCharge,
            Permissions.CashWithdraw,
            Permissions.CashCut,
            Permissions.CashRefund,
            Permissions.CashSettings,
            Permissions.KitchenView,
            Permissions.KitchenManage,
            Permissions.InventoryView,
            Permissions.InventoryAdjust,
            Permissions.MenuView,
            Permissions.MenuEdit,
            Permissions.PurchasingRequest,
            Permissions.PurchasingView,
            Permissions.PurchasingBuy,
            Permissions.PurchasingSuppliers,
            Permissions.PromosManage,
            Permissions.InvoicingIssue,
            Permissions.InvoicingGlobal,
            Permissions.InvoicingCancel,
            Permissions.InvoicingSettings,
            Permissions.ReportsView,
            Permissions.OnlineManage,
            Permissions.AuditView,
            Permissions.SecurityManage,
            Permissions.UsersManage,
            Permissions.SettingsManage,
            Permissions.BranchesManage,
            Permissions.BillingManage
        ]),
        ("Gerente",
        [
            Permissions.PlatformRemoteLogin,
            Permissions.PlatformDevices,
            Permissions.OrdersView,
            Permissions.PosTake,
            Permissions.PosCancel,
            Permissions.PosDiscount,
            Permissions.PosReprint,
            Permissions.PosTransfer,
            Permissions.CashView,
            Permissions.CashCharge,
            Permissions.CashWithdraw,
            Permissions.CashCut,
            Permissions.CashRefund,
            Permissions.CashSettings,
            Permissions.KitchenView,
            Permissions.KitchenManage,
            Permissions.InventoryView,
            Permissions.InventoryAdjust,
            Permissions.MenuView,
            Permissions.MenuEdit,
            Permissions.PurchasingRequest,
            Permissions.PurchasingView,
            Permissions.PurchasingBuy,
            Permissions.PurchasingSuppliers,
            Permissions.PromosManage,
            Permissions.InvoicingIssue,
            Permissions.InvoicingGlobal,
            Permissions.InvoicingCancel,
            Permissions.ReportsView,
            Permissions.OnlineManage,
            Permissions.AuditView,
            Permissions.SecurityManage,
            Permissions.SettingsManage
        ]),
        ("Cajero",
        [
            Permissions.OrdersView,
            Permissions.PosTake,
            Permissions.CashView,
            Permissions.CashCharge,
            Permissions.CashWithdraw,
            Permissions.CashCut,
            Permissions.InvoicingIssue,
            Permissions.OnlineManage
        ]),
        ("Mesero",
        [
            Permissions.OrdersView,
            Permissions.PosTake
        ]),
        ("Chef",
        [
            Permissions.OrdersView,
            Permissions.KitchenView,
            Permissions.KitchenManage,
            Permissions.InventoryView,
            Permissions.InventoryAdjust,
            Permissions.MenuView,
            Permissions.MenuEdit,
            Permissions.PurchasingRequest,
            Permissions.PurchasingView
        ]),
        ("Compras",
        [
            Permissions.InventoryView,
            Permissions.PurchasingView,
            Permissions.PurchasingBuy,
            Permissions.PurchasingSuppliers
        ])
    ];

    // The roster and PINs are the approved public demo fixtures.
    private static readonly (int Id, string Name, int Role, string Pin, bool Active)[] Users =
    [
        (1, "Andrea Ruiz", 1, "4821", true),
        (2, "Diana Cota", 2, "3307", true),
        (3, "Luis Ortega", 3, "5512", true),
        (4, "Karla Méndez", 3, "7734", true),
        (5, "Ramón Íñiguez", 4, "9045", true),
        (6, "Toño Valdez", 5, "6181", true),
        (7, "Paty Leyva", 3, "2268", true),
        (8, "Sergio Amador", 0, "1190", true),
        (9, "Brenda Soto", 3, "4410", false)
    ];

    internal static bool IsRequested(string[] args) => args is ["seed-demo"];

    internal static async Task<bool> RunAsync(string environment, string? connectionString, CancellationToken ct = default)
    {
        if (!string.Equals(environment, Environments.Development, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Demo seeding requires the Development environment.");
        }
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Set ConnectionStrings__Resto to the development database.");
        }

        var options = new DbContextOptionsBuilder().UseNpgsql(connectionString).Options;
        await using var db = new AppDbContext(options, new SeedActor());
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        await db.Database.ExecuteSqlInterpolatedAsync($"SELECT pg_advisory_xact_lock({SeedLock})", ct);

        // The restaurant and all its seed rows are inserted in the same transaction.
        var existing = await db.Restaurants.AsNoTracking().SingleOrDefaultAsync(ct);
        if (existing is not null)
        {
            if (existing.CreatedBy != ActorId)
            {
                throw new InvalidOperationException("The demo restaurant ID already belongs to existing data.");
            }

            await transaction.CommitAsync(ct);
            return false;
        }

        db.Organizations.Add(new Organization
        {
            Id = OrganizationId,
            Name = "Mariscos El Faro",
            LegalName = "Mariscos El Faro"
        });
        db.Restaurants.Add(new Restaurant
        {
            Id = RestaurantId,
            OrganizationId = OrganizationId,
            Name = "Mariscos El Faro",
            BranchName = "Ensenada, B.C."
        });
        db.AddRange(CreateRoster());
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return true;
    }

    internal static IEnumerable<TenantEntity> CreateRoster()
    {
        var hasher = new PasswordHasher<User>();
        for (var roleIndex = 0; roleIndex < Roles.Length; roleIndex++)
        {
            var definition = Roles[roleIndex];
            var role = new Role(OrganizationId, RestaurantId)
            {
                Id = SeedId(10 + roleIndex),
                Name = definition.Name,
                IsSystem = roleIndex == 0
            };
            yield return role;

            for (var grantIndex = 0; grantIndex < definition.Grants.Length; grantIndex++)
            {
                yield return new RolePermission
                {
                    Id = SeedId(1000 + roleIndex * 100 + grantIndex),
                    OrganizationId = OrganizationId,
                    RestaurantId = RestaurantId,
                    RoleId = role.Id,
                    Permission = definition.Grants[grantIndex]
                };
            }
        }

        foreach (var member in Users)
        {
            var user = new User
            {
                Id = SeedId(100 + member.Id),
                OrganizationId = OrganizationId,
                RestaurantId = RestaurantId,
                FullName = member.Name,
                Initials = string.Concat(member.Name.Split(' ').Select(part => part[0])),
                RoleId = SeedId(10 + member.Role),
                IsActive = member.Active,
                PinHash = string.Empty
            };
            user.PinHash = hasher.HashPassword(user, member.Pin);
            yield return user;
        }
    }

    private static Guid SeedId(int suffix) => Guid.Parse($"0192f2fe-0000-7008-8000-{suffix:D12}");

    // This identity exists only in the context owned by the explicit seed command.
    private sealed class SeedActor : ICurrentUser
    {
        public Guid UserId => ActorId;
        public Guid OrganizationId => DevSeed.OrganizationId;
        public Guid RestaurantId => DevSeed.RestaurantId;
        public Guid? DeviceId => null;
        public bool HasPermission(string permission) => false;
    }
}
