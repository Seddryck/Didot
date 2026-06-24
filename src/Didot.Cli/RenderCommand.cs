using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Didot.Core;
using Didot.Core.SourceParsers;
using Didot.Core.TemplateEngines;
using Microsoft.Extensions.Logging;

namespace Didot.Cli;
public class RenderCommand : RootCommand
{
    public RenderCommand(RenderOptions options, ILogger<RenderCommand>? logger = null)
        : base("Didot Command Line Interface")
        => Configure(this, options, logger);

    public static void Configure(Command command, RenderOptions options, ILogger<RenderCommand>? logger = null, RenderCommandHandler? commandHandler = null)
    {
        options.EngineExtensions.DefaultValueFactory = _ => [];
        options.ParserExtensions.DefaultValueFactory = _ => [];
        options.ParserParams.DefaultValueFactory = _ => [];
        options.Sources.DefaultValueFactory = _ => [];

        command.Options.Add(options.Template);
        command.Options.Add(options.Engine);
        command.Options.Add(options.EngineExtensions);
        command.Options.Add(options.Sources);
        command.Options.Add(options.StdIn);
        command.Options.Add(options.Parser);
        command.Options.Add(options.ParserExtensions);
        command.Options.Add(options.ParserParams);
        command.Options.Add(options.Output);

        command.Validators.Add(result =>
        {
            if (HasSubcommand(result))
                return;

            var template = result.GetValue(options.Template);
            if (string.IsNullOrWhiteSpace(template))
                result.AddError("Option '--template' is required.");
        });

        command.Validators.Add(result => {
            if (HasSubcommand(result))
                return;

            var stdIn = result.GetValue(options.StdIn);

            var sourcesResult = result.GetResult(options.Sources);
            var hasSources = sourcesResult is not null
                && !sourcesResult.Implicit
                && sourcesResult.Tokens.Count > 0;

            if (stdIn && hasSources)
            {
                result.AddError(
                    "The --stdin option cannot be provided together with the --source option.");
            }
        });

        command.Validators.Add(result => {
            if (HasSubcommand(result))
                return;

            var stdIn = result.GetValue(options.StdIn);
            var parser = result.GetValue(options.Parser) ?? string.Empty;
            if (stdIn && string.IsNullOrEmpty(parser))
                result.AddError("The --parser option is required when using --stdin to specify the input source.");
        });

        command.Validators.Add(result => {
            if (HasSubcommand(result))
                return;

            var stdIn = result.GetValue(options.StdIn);
            var sourcesResult = result.GetResult(options.Sources);
            var hasSources = sourcesResult is not null
                && !sourcesResult.Implicit
                && sourcesResult.Tokens.Count > 0;

            if (!stdIn && !hasSources)
                result.AddError("The --stdin option is required when not using --source to specify the input file(s).");
        });

        command.SetAction(parseResult =>
        {
            try
            {
                var handler = commandHandler ?? new RenderCommandHandler(logger);

                handler.Execute(
                    parseResult.GetValue(options.Template)!,
                    parseResult.GetValue(options.Engine)!,
                    parseResult.GetValue(options.EngineExtensions) ?? [],
                    parseResult.GetValue(options.Sources) ?? [],
                    parseResult.GetValue(options.Parser)!,
                    parseResult.GetValue(options.ParserExtensions) ?? [],
                    parseResult.GetValue(options.ParserParams) ?? [],
                    parseResult.GetValue(options.Output)!
                );

                return (int)CliExitCode.Success;
            }
            catch (CliException ex)
            {
                Console.Error.WriteLine(ex.Message);
                return (int)ex.ExitCode;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Unexpected error: {ex.Message}");
                return (int)CliExitCode.InternalError;
            }
        });
    }

    private static bool HasSubcommand(CommandResult result)
        => result.Children.Any(x => x is CommandResult);
}

