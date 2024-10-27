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
        options.EngineExtensions.SetDefaultValue(new Dictionary<string, string>());
        options.ParserExtensions.SetDefaultValue(new Dictionary<string, string>());
        options.Sources.SetDefaultValue(new Dictionary<string, string>());

        AddOption(options.Template);
        AddOption(options.Engine);
        AddOption(options.EngineExtensions);
        AddOption(options.Sources);
        AddOption(options.StdIn);
        AddOption(options.Parser);
        AddOption(options.ParserExtensions);
        AddOption(options.Output);

        AddValidator(result =>
        {
            var stdInProvided = result.GetValueForOption(options.StdIn);
            var sourcesProvided = false;
            try
            {
                sourcesProvided = result.GetValueForOption(options.Sources) is not null
                                    && result.GetValueForOption(options.Sources)!.Any();
            }
            catch { }

            if (stdInProvided && sourcesProvided)
                result.ErrorMessage = "The --stdin option cannot be provided together with the --source option.";
        });

        AddValidator(result =>
        {
            var stdInProvided = result.GetValueForOption(options.StdIn);
            var parserProvided = !string.IsNullOrEmpty(result.GetValueForOption(options.Parser));

            if (stdInProvided && !parserProvided)
                result.ErrorMessage = "The --parser option is required when using --stdin to specify the input source.";
        });

        AddValidator(result =>
        {
            var stdInProvided = result.GetValueForOption(options.StdIn);
            try
            {
                var sourceProvided = result.GetValueForOption(options.Sources) is null || result.GetValueForOption(options.Sources)!.Any();
                if (!stdInProvided && !sourceProvided)
                    result.ErrorMessage = "The --stdin option is required when not using --source to specify the input file(s).";
            }
            catch { }
        });

        this.SetHandler(new RenderCommandHandler(logger).Execute
            , options.Template
            , options.Engine
            , options.EngineExtensions
            , options.Sources
            , options.Parser
            , options.ParserExtensions
            , options.Output);
    }
}
