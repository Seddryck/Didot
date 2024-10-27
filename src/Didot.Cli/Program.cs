using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;
using System.Text;
using Didot.Core;
using Didot.Core.SourceParsers;
using Didot.Core.TemplateEngines;
using HandlebarsDotNet;

namespace Didot.Cli;
public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var renderOptions = new RenderOptions();
        var rootCommand = new RenderCommand(renderOptions);
        return await rootCommand.InvokeAsync(args);
    }
}
