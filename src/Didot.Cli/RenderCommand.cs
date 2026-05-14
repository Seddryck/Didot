using System;
using System.CommandLine;
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
    {
        options.EngineExtensions.DefaultValueFactory = _ => [];
        options.ParserExtensions.DefaultValueFactory = _ => [];
        options.ParserParams.DefaultValueFactory = _ => [];
        options.Sources.DefaultValueFactory = _ => [];

        Options.Add(options.Template);
        Options.Add(options.Engine);
        Options.Add(options.EngineExtensions);
        Options.Add(options.Sources);
        Options.Add(options.StdIn);
        Options.Add(options.Parser);
        Options.Add(options.ParserExtensions);
        Options.Add(options.ParserParams);
        Options.Add(options.Output);

        Validators.Add(result => {
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

        Validators.Add(result => {
            var stdIn = result.GetValue(options.StdIn);
            var parser = result.GetValue(options.Parser) ?? string.Empty;
            if (stdIn && string.IsNullOrEmpty(parser))
                result.AddError("The --parser option is required when using --stdin to specify the input source.");
        });

        Validators.Add(result => {
            var stdIn = result.GetValue(options.StdIn);
            var sourcesResult = result.GetResult(options.Sources);
            var hasSources = sourcesResult is not null
                && !sourcesResult.Implicit
                && sourcesResult.Tokens.Count > 0;

            if (!stdIn && !hasSources)
                result.AddError("The --stdin option is required when not using --source to specify the input file(s).");
        });

        this.SetAction(parseResult =>
        {
            var handler = new RenderCommandHandler(logger);

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
        });
    }
}

