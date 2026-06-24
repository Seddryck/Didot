using System.CommandLine;
using System.CommandLine.Parsing;

namespace Didot.Cli;

public class RegisterExtensionOptions
{
    public Argument<string> Reference { get; } = new("reference")
    {
        Description = "Extension reference (id/name, assembly path, or directory path).",
        Arity = ArgumentArity.ExactlyOne,
    };

    public Option<string> Name { get; } = new("--name")
    {
        Description = "Friendly name override for the registered extension.",
        Arity = ArgumentArity.ZeroOrOne,
    };
}
