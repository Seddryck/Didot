namespace Didot.Cli;

public enum CliExitCode
{
    Success = 0,
    InvalidInput = 2,
    NotFound = 3,
    LoadFailure = 4,
    InternalError = 99,
}
