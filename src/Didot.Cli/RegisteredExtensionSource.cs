namespace Didot.Cli;

public sealed record RegisteredExtensionSource(
    string Id,
    string Name,
    string AssemblyPath
);
