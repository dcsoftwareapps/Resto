namespace Resto.Domain.Platform;

public abstract class TenantEntity : Entity
{
    public Guid OrganizationId { get; set; }
    public Guid RestaurantId { get; set; }
}
