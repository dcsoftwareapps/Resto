using System.Security.Claims;
using Resto.Contracts;
using Resto.Domain.Platform.Abstractions;

namespace Resto.Api.Security;

public sealed class HttpCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public Guid UserId => RequireScope().UserId;
    public Guid OrganizationId => RequireScope().OrganizationId;
    public Guid RestaurantId => RequireScope().RestaurantId;
    public Guid? DeviceId => RequireScope().DeviceId;

    public bool HasPermission(string permission) =>
        Permissions.All.Contains(permission) &&
        ReadScope() is { } scope &&
        scope.Identity.HasClaim(claim => claim.Type == "perm" && claim.Value == permission);

    private Scope RequireScope() => ReadScope() ??
        throw new InvalidOperationException("An authenticated user with a valid tenant scope is required.");

    private Scope? ReadScope()
    {
        var identities = accessor.HttpContext?.User.Identities.Where(identity => identity.IsAuthenticated).ToArray();
        if (identities is not [var identity] ||
            ReadId(identity, "sub") is not { } userId ||
            ReadId(identity, "org") is not { } organizationId ||
            ReadId(identity, "rest") is not { } restaurantId)
        {
            return null;
        }

        var deviceId = ReadId(identity, "dev");
        if (deviceId is null && identity.HasClaim(claim => claim.Type == "dev"))
        {
            return null;
        }

        return new Scope(identity, userId, organizationId, restaurantId, deviceId);
    }

    private static Guid? ReadId(ClaimsIdentity identity, string claimType)
    {
        var claims = identity.Claims.Where(claim => claim.Type == claimType).ToArray();
        return claims is [var claim] && Guid.TryParse(claim.Value, out var id) && id != Guid.Empty
            ? id
            : null;
    }

    private sealed record Scope(ClaimsIdentity Identity, Guid UserId, Guid OrganizationId, Guid RestaurantId, Guid? DeviceId);
}
