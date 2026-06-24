namespace Didot.Cli;

public abstract class CliException : Exception
{
    public CliExitCode ExitCode { get; }

    protected CliException(CliExitCode exitCode, string message, Exception? innerException = null)
        : base(message, innerException)
        => ExitCode = exitCode;
}
