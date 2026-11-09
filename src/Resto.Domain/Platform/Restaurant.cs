namespace Resto.Domain.Platform;

public sealed class Restaurant : Entity
{
    public Guid OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string TimeZone { get; set; } = "America/Tijuana";
    public TimeOnly BusinessDayCutoff { get; set; } = new(4, 0);
    public int NextOrderNumber { get; set; } = 1;
}
