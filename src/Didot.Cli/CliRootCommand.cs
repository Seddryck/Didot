using System.CommandLine;
using Microsoft.Extensions.Logging;

namespace Didot.Cli;

public class CliRootCommand : RootCommand
{
    public CliRootCommand(
        RenderOptions renderOptions,
        ExtensionsCommand extensionsCommand,
        RenderCommandHandler? renderCommandHandler = null,
        ILogger<RenderCommand>? logger = null)
        : base("Didot Command Line Interface")
    {
        RenderCommand.Configure(this, renderOptions, logger, renderCommandHandler);
        Subcommands.Add(extensionsCommand);
    }
}
