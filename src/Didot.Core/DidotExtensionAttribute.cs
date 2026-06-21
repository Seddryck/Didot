namespace Didot.Core;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public sealed class DidotExtensionAttribute(string id, string name) : Attribute
{
    public string Id { get; } = string.IsNullOrWhiteSpace(id)
        ? throw new ArgumentException("Extension id cannot be null or whitespace.", nameof(id))
        : id.Trim();

    public string Name { get; } = string.IsNullOrWhiteSpace(name)
        ? throw new ArgumentException("Extension name cannot be null or whitespace.", nameof(name))
        : name.Trim();

    public string? Version { get; init; }
}
