namespace Resto.Domain.Platform;

[Audited("Platform", "la organización", nameof(Name), nameof(LegalName), nameof(IsActive))]
public sealed class Organization : Entity
{
    public string Name { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
}
