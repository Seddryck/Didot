using Didot.Core;

namespace Didot.Cli;

public static class ExtensionLoadExceptionExtensions
{
    public static CliExitCode ToCliExitCode(this ExtensionException exception)
        => exception switch
        {
            RegistrationFileNotFoundException => CliExitCode.NotFound,
            RegistrationFileInvalidException => CliExitCode.LoadFailure,
            ExtensionSourceNotFoundException => CliExitCode.NotFound,
            AssemblyLoadFailedException => CliExitCode.LoadFailure,
            ExtensionTypeNotFoundException => CliExitCode.LoadFailure,
            ExtensionTypeAmbiguousException => CliExitCode.LoadFailure,
            ExtensionInstantiationFailedException => CliExitCode.LoadFailure,
            _ => CliExitCode.InternalError,
        };
}
