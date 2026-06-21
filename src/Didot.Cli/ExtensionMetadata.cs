namespace Didot.Cli;

public sealed record ExtensionMetadata(
    string Id,
    string Name,
    string Version,
    string AssemblyPath,
    string AssemblyName
);
