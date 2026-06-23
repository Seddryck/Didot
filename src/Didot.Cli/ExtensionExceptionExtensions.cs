using Didot.Core;

namespace Didot.Cli;

public static class ExtensionExceptionExtensions
{
    public static ExtensionCliException ToCliException(this ExtensionException error)
        => new(
            error.GetType(),
            error.ToCliExitCode(),
            $"[{error.GetType().Name}] {error.Message}",
            error.SourcePath,
            error.InnerException);
}
