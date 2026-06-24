using System.CommandLine;

namespace Didot.Cli;

public class ExtensionsCommand : Command
{
    public ExtensionsCommand(RegisterExtensionCommand registerExtensionCommand)
        : base("extensions", "Manage Didot extensions.")
    {
        Subcommands.Add(registerExtensionCommand);
    }
}
