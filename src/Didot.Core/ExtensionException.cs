namespace Didot.Core;

public abstract class ExtensionException : Exception
{
    public string? SourcePath { get; }

    protected ExtensionException(
        string message,
        string? sourcePath = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        SourcePath = sourcePath;
    }

    protected static string WithSourcePath(string message, string? sourcePath)
        => string.IsNullOrWhiteSpace(sourcePath)
            ? message
            : $"{message} Source path: '{sourcePath}'.";
}

public sealed class RegistrationFileNotFoundException(
    string message,
    string? sourcePath = null,
    Exception? innerException = null)
    : ExtensionException(message, sourcePath, innerException)
{
    public RegistrationFileNotFoundException(string? sourcePath = null, Exception? innerException = null)
        : this(WithSourcePath("No extension registry file was found.", sourcePath), sourcePath, innerException)
    { }
}

public sealed class RegistrationFileInvalidException(
    string message,
    string? sourcePath = null,
    Exception? innerException = null)
    : ExtensionException(message, sourcePath, innerException)
{
    public RegistrationFileInvalidException(string? sourcePath = null, Exception? innerException = null)
        : this(WithSourcePath("The extension registry file is invalid.", sourcePath), sourcePath, innerException)
    { }
}

public sealed class ExtensionSourceNotFoundException(
    string message,
    string? sourcePath = null,
    Exception? innerException = null)
    : ExtensionException(message, sourcePath, innerException)
{
    public ExtensionSourceNotFoundException(string? sourcePath = null, Exception? innerException = null)
        : this(WithSourcePath("The extension source was not found.", sourcePath), sourcePath, innerException)
    { }
}

public sealed class AssemblyLoadFailedException(
    string message,
    string? sourcePath = null,
    Exception? innerException = null)
    : ExtensionException(message, sourcePath, innerException)
{
    public AssemblyLoadFailedException(string? sourcePath = null, Exception? innerException = null)
        : this(WithSourcePath("Unable to load extension assembly.", sourcePath), sourcePath, innerException)
    { }
}

public sealed class ExtensionTypeNotFoundException(
    string message,
    string? sourcePath = null,
    Exception? innerException = null)
    : ExtensionException(message, sourcePath, innerException)
{
    public ExtensionTypeNotFoundException(string? sourcePath = null, Exception? innerException = null)
        : this(WithSourcePath("No compatible extension type was found.", sourcePath), sourcePath, innerException)
    { }
}

public sealed class ExtensionTypeAmbiguousException(
    string message,
    string? sourcePath = null,
    Exception? innerException = null)
    : ExtensionException(message, sourcePath, innerException)
{
    public ExtensionTypeAmbiguousException(string? sourcePath = null, Exception? innerException = null)
        : this(WithSourcePath("Multiple compatible extension types were found.", sourcePath), sourcePath, innerException)
    { }
}

public sealed class ExtensionInstantiationFailedException(
    string message,
    string? sourcePath = null,
    Exception? innerException = null)
    : ExtensionException(message, sourcePath, innerException)
{
    public ExtensionInstantiationFailedException(string? sourcePath = null, Exception? innerException = null)
        : this(WithSourcePath("Failed to instantiate extension type.", sourcePath), sourcePath, innerException)
    { }
}
