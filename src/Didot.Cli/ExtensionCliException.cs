using Didot.Core;

namespace Didot.Cli;

public sealed class ExtensionCliException : CliException
{
    public Type ErrorType { get; }
    public string? SourcePath { get; }

    public ExtensionCliException(
        Type errorType,
        CliExitCode exitCode,
        string message,
        string? sourcePath = null,
        Exception? innerException = null)
        : base(exitCode, message, innerException)
    {
        ErrorType = errorType;
        SourcePath = sourcePath;
    }
}
