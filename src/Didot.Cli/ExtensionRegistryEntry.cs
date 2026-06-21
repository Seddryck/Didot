using System.Text.Json.Serialization;

namespace Didot.Cli;

public sealed class ExtensionRegistryEntry
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("assembly")]
    public required string Assembly { get; init; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; init; } = true;

    [JsonPropertyName("version")]
    public required string Version { get; init; }

    [JsonPropertyName("registeredAt")]
    public DateTimeOffset RegisteredAt { get; init; }
}
