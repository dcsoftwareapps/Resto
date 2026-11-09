namespace Resto.Domain.Platform.Abstractions;

/// <summary>Identity and authorized scope supplied by the authenticated caller.</summary>
public interface ICurrentUser
{
    Guid UserId { get; }
    Guid OrganizationId { get; }
    Guid RestaurantId { get; }
    Guid? DeviceId { get; }
    bool HasPermission(string permission);
}
