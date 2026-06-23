using Didot.Core;

namespace Didot.Cli;

public sealed record LoadedExtension(
    string Source,
    string TypeName,
    IPipelineExtensionHook Instance
);
