using System;
using System.CommandLine;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Didot.Cli;
public class Program
{
    public static async Task<int> Main(string[] args)
    {
        using var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
            })
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSimpleConsole(options =>
                {
                    options.SingleLine = true;
                    options.IncludeScopes = false;
                    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss.fff] ";
                });
                logging.SetMinimumLevel(LogLevel.None);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddLogging();
                services.AddTransient<RenderOptions>();
                services.AddTransient<RegisterExtensionOptions>();
                services.AddTransient<ExtensionMetadataReader>();
                services.AddTransient<ExtensionReferenceResolver>();
                services.AddTransient<InstallationExtensionRegistryRepository>();
                services.AddTransient<InstallationExtensionSourceResolver>();
                services.AddTransient<ExtensionAssemblyLoader>();
                services.AddTransient<RenderCommandHandler>();
                services.AddTransient<RegisterExtensionCommandHandler>();
                services.AddTransient<RegisterExtensionCommand>();
                services.AddTransient<ExtensionsCommand>();
                services.AddTransient<RootCommand>(provider =>
                    new CliRootCommand(
                        provider.GetRequiredService<RenderOptions>(),
                        provider.GetRequiredService<ExtensionsCommand>(),
                        provider.GetRequiredService<RenderCommandHandler>(),
                        provider.GetRequiredService<ILogger<RenderCommand>>()
                    ));
            })
            .Build();

        var logger = host.Services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation($"Didot Command Line Interface: version {Assembly.GetExecutingAssembly().GetName().Version}");

        try
        {
            var renderCommand = host.Services.GetRequiredService<RootCommand>();
            var parseResult = renderCommand.Parse(args);
            return await parseResult.InvokeAsync();
        }
        catch (CliException ex)
        {
            await Console.Error.WriteLineAsync(ex.Message);
            return (int)ex.ExitCode;
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"Unexpected error: {ex.Message}");
            return (int)CliExitCode.InternalError;
        }
    }
}
