namespace Didot.Cli;

public sealed class RenderCliException : CliException
{
    public string? SourcePath { get; }

    public RenderCliException(
        CliExitCode exitCode,
        string message,
        string? sourcePath = null,
        Exception? innerException = null)
        : base(exitCode, message, innerException)
        => SourcePath = sourcePath;
}
