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

        Validators.Add(result =>
        {
            var stdInProvided = result.GetValue(options.StdIn);
            var sourcesProvided = false;
            try
            {
                sourcesProvided = (result.GetValue(options.Sources)?.Any() ?? false);
            }
            catch
            { }

            if (stdInProvided && sourcesProvided)
                result.AddError("The --stdin option cannot be provided together with the --source option.");
        });

        Validators.Add(result =>
        {
            var stdInProvided = result.GetValue(options.StdIn);
            var parserProvided = !string.IsNullOrEmpty(result.GetValue(options.Parser));

            if (stdInProvided && !parserProvided)
                result.AddError("The --parser option is required when using --stdin to specify the input source.");
        });

        Validators.Add(result =>
        {
            var stdInProvided = result.GetValue(options.StdIn);

            var sourcesProvided = false;
            try
            {
                sourcesProvided = (result.GetValue(options.Sources)?.Any() ?? false);
            }
            catch
            { }

            if (!stdInProvided && !sourcesProvided)
                result.AddError("The --stdin option is required when not using --source to specify the input file(s).");
        });

        SetAction(parsed => new RenderCommandHandler(logger).Execute(
            parsed.GetRequiredValue(options.Template)
            , parsed.GetValue(options.Engine) ?? string.Empty
            , parsed.GetRequiredValue(options.EngineExtensions)
            , parsed.GetRequiredValue(options.Sources)
            , parsed.GetValue(options.Parser) ?? string.Empty
            , parsed.GetRequiredValue(options.ParserExtensions)
            , parsed.GetRequiredValue(options.ParserParams)
            , parsed.GetValue(options.Output) ?? string.Empty));
    }
}
