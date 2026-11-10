namespace Resto.Domain.Platform;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AuditedAttribute(string module, string subject, params string[] properties) : Attribute
{
    public string Module { get; } = module;
    public string Subject { get; } = subject;
    public IReadOnlyList<string> Properties { get; } = properties;
}
