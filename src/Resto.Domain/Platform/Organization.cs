namespace Resto.Domain.Platform;

public sealed class Organization : Entity
{
    public string Name { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
}
