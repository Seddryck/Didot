using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.TemplateEngines;
using DotLiquid.Tags;
using HandlebarsDotNet;
using NUnit.Framework;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Didot.Cli.Testing;

public class ProgramTests
{
    private static readonly string[] Templates = { "scriban", "liquid", "hbs", "smart", "st", "morestachio" };
    private static readonly string[] DataSets = { "yaml", "json", "xml", "csv" };
    private static readonly string[] Cases = { "employees", "organization" };

    private TextWriter OriginalOutput { get; set; }
    private Stream OutputStream { get; set; }
    private StreamWriter OutputWriter { get; set; }

    private TextWriter OriginalError { get; set; }
    private Stream ErrorStream { get; set; }
    private StreamWriter ErrorWriter { get; set; }

    private string ReadErrorStream()
    {
        ErrorStream.Position = 0;
        using var reader = new StreamReader(ErrorStream);
        return reader.ReadToEnd();
    }

    private string ReadOutputStream()
    {
        OutputStream.Position = 0;
        using var reader = new StreamReader(OutputStream);
        return reader.ReadToEnd().Standardize();
    }

    private static string GetInstallationRegistryPath()
        => Path.Combine(AppContext.BaseDirectory, Cli.InstallationExtensionRegistryRepository.RegistryFileName);

    private static async Task ExecuteWithIsolatedRegistry(Func<string, Task> action)
    {
        var registryPath = GetInstallationRegistryPath();
        var backupPath = string.Empty;

        if (File.Exists(registryPath))
        {
            backupPath = $"{registryPath}.{Guid.NewGuid():N}.bak";
            File.Copy(registryPath, backupPath, true);
            File.Delete(registryPath);
        }

        try
        {
            await action(registryPath);
        }
        finally
        {
            if (File.Exists(registryPath))
                File.Delete(registryPath);

            if (!string.IsNullOrEmpty(backupPath) && File.Exists(backupPath))
                File.Move(backupPath, registryPath, true);
        }
    }

    [SetUp]
    public void SetUp()
    {
        OriginalOutput = Console.Out;
        OutputStream = new MemoryStream();
        OutputWriter = new(OutputStream)
        {
            AutoFlush = true
        };
        Console.SetOut(OutputWriter);

        OriginalError = Console.Error;
        ErrorStream = new MemoryStream();
        ErrorWriter = new(ErrorStream)
        {
            AutoFlush = true
        };
        Console.SetError(ErrorWriter);
    }

    [TearDown]
    public void TearDown()
    {
        OutputWriter.Dispose();
        OutputStream.Dispose();
        Console.SetOut(OriginalOutput);

        ErrorWriter.Dispose();
        ErrorStream.Dispose();
        Console.SetError(OriginalError);
    }

    [Test]
    [NonParallelizable]
    public async Task Main_ExtensionsRegister_WritesInstallationRegistry()
        => await ExecuteWithIsolatedRegistry(async registryPath =>
        {
            var extensionPath = typeof(Extensions.PipelineHookForE2eTests).Assembly.Location;

            var exitCode = await Program.Main([
                "extensions", "register", extensionPath, "--name", "Hook E2E"
            ]);

            Assert.That(exitCode, Is.Zero, message: ReadErrorStream());
            Assert.That(File.Exists(registryPath), Is.True);

            var repository = new Cli.InstallationExtensionRegistryRepository(registryPath);
            var entries = repository.ReadAll();
            Assert.That(entries, Has.Count.EqualTo(1));
            Assert.That(entries[0].Assembly, Is.EqualTo(Path.GetFullPath(extensionPath)));
            Assert.That(entries[0].Name, Is.EqualTo("Hook E2E"));
        });

    [Test]
    [NonParallelizable]
    public async Task Main_Render_WithRegisteredExtension_AppliesHook()
        => await ExecuteWithIsolatedRegistry(async registryPath =>
        {
            var repository = new Cli.InstallationExtensionRegistryRepository(registryPath);
            repository.Register(new Cli.ExtensionRegistryEntry
            {
                Id = "didot.cli.testing.extension",
                Name = "Cli Testing Extension",
                Assembly = typeof(Extensions.PipelineHookForE2eTests).Assembly.Location,
                Enabled = true,
                Version = "1.0.0",
                RegisteredAt = DateTimeOffset.UtcNow,
            });

            var directory = Path.Combine(Path.GetTempPath(), $"didot-e2e-hook-{Guid.NewGuid():N}");
            Directory.CreateDirectory(directory);

            try
            {
                File.WriteAllText(Path.Combine(directory, "template.hbs"), "{{model.fullname}}");
                File.WriteAllText(Path.Combine(directory, "source.json"), "{\"firstname\":\"Ada\",\"lastname\":\"Lovelace\"}");

                var originalCurrentDirectory = Directory.GetCurrentDirectory();
                Directory.SetCurrentDirectory(directory);
                try
                {
                    var exitCode = await Program.Main([
                        "-t", "template.hbs",
                        "-s", "source.json",
                        "-r", "json"
                    ]);

                    Assert.That(exitCode, Is.Zero, message: ReadErrorStream());
                    Assert.That(ReadOutputStream(), Is.EqualTo("Ada Lovelace"));
                }
                finally
                {
                    Directory.SetCurrentDirectory(originalCurrentDirectory);
                }
            }
            finally
            {
                if (Directory.Exists(directory))
                    Directory.Delete(directory, true);
            }
        });

    [Test]
    [NonParallelizable]
    public async Task Main_Render_WithRegisteredThrowingExtension_Fails()
        => await ExecuteWithIsolatedRegistry(async registryPath =>
        {
            var repository = new Cli.InstallationExtensionRegistryRepository(registryPath);
            repository.Register(new Cli.ExtensionRegistryEntry
            {
                Id = "didot.cli.testing.throwing",
                Name = "Cli Testing Throwing Extension",
                Assembly = typeof(Extensions.PipelineHookForE2eTests).Assembly.Location,
                Enabled = true,
                Version = "1.0.0",
                RegisteredAt = DateTimeOffset.UtcNow,
            });

            var exitCode = await Program.Main([
                "-t", "template/employees.hbs",
                "-s", "data/employees.json",
                "-r", "json"
            ]);

            Assert.That(exitCode, Is.Not.Zero);
            Assert.That(ReadErrorStream(), Does.Contain("hook was applied"));
        });

    [Test]
    [NonParallelizable]
    public async Task Main_Render_WithDisabledExtension_DoesNotApplyHook()
        => await ExecuteWithIsolatedRegistry(async registryPath =>
        {
            var repository = new Cli.InstallationExtensionRegistryRepository(registryPath);
            repository.Register(new Cli.ExtensionRegistryEntry
            {
                Id = "didot.cli.testing.extension",
                Name = "Cli Testing Extension",
                Assembly = typeof(Extensions.PipelineHookForE2eTests).Assembly.Location,
                Enabled = false,
                Version = "1.0.0",
                RegisteredAt = DateTimeOffset.UtcNow,
            });

            var exitCode = await Program.Main([
                "-t", "template/employees.hbs",
                "-s", "data/employees.json",
                "-r", "json"
            ]);

            Assert.That(exitCode, Is.Zero, message: ReadErrorStream());

            var expected = File.ReadAllText(Path.Combine("Expectation", "employees.txt")).Standardize();
            Assert.That(ReadOutputStream(), Is.EqualTo(expected));
        });

    [Test]
    [NonParallelizable]
    public async Task Main_Render_WithMissingExtensionSource_FailsWithDiagnosticCode()
        => await ExecuteWithIsolatedRegistry(async registryPath =>
        {
            var repository = new Cli.InstallationExtensionRegistryRepository(registryPath);
            repository.Register(new Cli.ExtensionRegistryEntry
            {
                Id = "missing.extension",
                Name = "Missing extension",
                Assembly = Path.Combine(Path.GetTempPath(), $"missing-{Guid.NewGuid():N}.dll"),
                Enabled = true,
                Version = "1.0.0",
                RegisteredAt = DateTimeOffset.UtcNow,
            });

            var args = new[]
            {
                "-t", "template/employees.hbs",
                "-s", "data/employees.json",
                "-r", "json"
            };

            var exitCode = await Program.Main(args);

            Assert.That(exitCode, Is.EqualTo((int)Cli.CliExitCode.NotFound));
            Assert.That(ReadErrorStream(), Does.Contain("[ExtensionSourceNotFoundException]"));
        });

    [Test, Combinatorial]
    public async Task Main_StdInStdOut_Successful(
            [ValueSource(nameof(Templates))] string engine,
            [ValueSource(nameof(DataSets))] string data,
            [ValueSource(nameof(Cases))] string caseId)
    {
        if (engine == "morestachio" && data == "yaml")
            Assert.Ignore("Morestachio doesn't support Yaml data sources at this moment.");

        using var source = new StreamReader(Path.Combine("data", $"{caseId}.{data}"));
        Console.SetIn(source);

        var args = new string[]
        {
            $"-t", $"template/{caseId}.{engine}",
            $"-r", $"{data}",
            $"--stdin"
        };
        var exitCode = await Program.Main(args);
        Assert.That(exitCode, Is.EqualTo(0), message: ReadErrorStream());

        var expected = File.ReadAllText(Path.Combine("Expectation", $"{caseId}.txt")).Standardize();
        Assert.That(ReadOutputStream(), Is.EqualTo(expected));
    }

    [Test, Combinatorial]
    public async Task Main_SourceFileOutputFile_Successful(
            [ValueSource(nameof(Templates))] string engine,
            [ValueSource(nameof(DataSets))] string data,
            [ValueSource(nameof(Cases))] string caseId)
    {
        if (engine == "morestachio" && data == "yaml")
            Assert.Ignore("Morestachio doesn't support Yaml data sources at this moment.");

        var args = new string[]
        {
            $"-t", $"template/{caseId}.{engine}",
            $"-s", $"data/{caseId}.{data}",
            $"-o", $"output-{caseId}-{engine}-{data}.txt"
        };
        var exitCode = await Program.Main(args);
        Assert.That(exitCode, Is.EqualTo(0), message: ReadErrorStream());

        var output = File.ReadAllText($"output-{caseId}-{engine}-{data}.txt").Standardize();
        var expected = File.ReadAllText(Path.Combine("Expectation", $"{caseId}.txt")).Standardize();
        Assert.That(output, Is.EqualTo(expected));
    }

    [TestCase("hbs", "tsv", "organization", "\t")]
    [TestCase("hbs", "tsv", "organization", "Tab")]
    [TestCase("hbs", "tsv", "organization", "TAB")]
    public async Task Main_SourceFileOutputFileWithParameters_Successful(
            string engine,
            string data,
            string caseId,
            string delimiter)

    {
        var args = new string[]
        {
            $"-t", $"template/{caseId}.{engine}",
            $"-s", $"data/{caseId}.{data}",
            $"-X", "tsv:csv",
            $"-P", $"tsv@delimiter:{delimiter};tsv@commentChar:#",
            $"-o", $"output-{caseId}-{engine}-{data}.txt"
        };
        var exitCode = await Program.Main(args);
        Assert.That(exitCode, Is.EqualTo(0), message: ReadErrorStream());

        var output = File.ReadAllText($"output-{caseId}-{engine}-{data}.txt").Standardize();
        var expected = File.ReadAllText(Path.Combine("Expectation", $"{caseId}.txt")).Standardize();
        Assert.That(output, Is.EqualTo(expected));
    }

    [Test]
    public async Task Main_MissingStdInAndSource_Failure()
    {
        var args = new string[]
        {
            $"-t", "template/employees.hbs",
            $"-o", "failure.txt"
        };
        var exitCode = await Program.Main(args);
        Assert.That(exitCode, Is.Not.EqualTo(0));
        Assert.That(ReadErrorStream(), Does.StartWith("The --stdin option is required when not using --source to specify the input file(s)."));
    }

    [Test]
    public async Task Main_MissingParserAndSource_Failure()
    {
        var args = new string[]
        {
            $"-t", "template/employees.hbs",
            $"--stdin",
            $"-o", "failure.txt"
        };
        var exitCode = await Program.Main(args);
        Assert.That(exitCode, Is.Not.EqualTo(0));
        Assert.That(ReadErrorStream(), Does.StartWith("The --parser option is required when using --stdin to specify the input source."));
    }

    [Test]
    public async Task Main_MissingTemplate_Failure()
    {
        var args = new string[]
        {
            $"-r", $"json",
            "--stdin",
            $"-o", $"failure.txt"
        };
        var exitCode = await Program.Main(args);
        Assert.That(exitCode, Is.Not.Zero);
        Assert.That(ReadErrorStream(), Does.StartWith("Option '--template' is required."));
    }

    [Test]
    public async Task Main_ForcedEngine_Success()
    {
        var args = new string[]
        {
            $"-ttemplate/employees.liquid",
            $"-sdata/employees.json",
            $"-efluid"
        };
        var exitCode = await Program.Main(args);
        Assert.That(exitCode, Is.Zero, message: ReadErrorStream());

        var expected = File.ReadAllText(Path.Combine("Expectation", $"employees.txt")).Standardize();
        Assert.That(ReadOutputStream(), Is.EqualTo(expected));
    }

    [Test]
    [TestCase("-x", ':')]
    [TestCase("--engine-extension", ':')]
    public async Task Main_AddNewEngineExtension_Success(string token, char delimiter)
    {
        string extension = "txt";
        string engineTag = "handlebars";

        var args = new string[]
        {
            $"-t", $"template/employees.{extension}",
            $"{token}", $"{extension}{delimiter}{engineTag}",
            $"-s", $"data/employees.json",
            $"-r", $"json"
        };
        var exitCode = await Program.Main(args);
        Assert.That(exitCode, Is.EqualTo(0), message: ReadErrorStream());

        var expected = File.ReadAllText(Path.Combine("Expectation", $"employees.txt")).Standardize();
        Assert.That(ReadOutputStream(), Is.EqualTo(expected));
    }

    [Test]
    [TestCase("liquid", "fluid")]
    public async Task Main_ReplaceExistingEngineExtension_Success(string extension, string engineTag)
    {
        using var source = new StreamReader(Path.Combine("data", $"employees.json"));
        Console.SetIn(source);

        var args = new string[]
        {
            $"-t", $"template/employees.{extension}",
            $"-x", $"{extension}:{engineTag}",
            $"-r", $"json",
            $"--stdin"
        };
        var exitCode = await Program.Main(args);
        Assert.That(exitCode, Is.EqualTo(0), message: ReadErrorStream());

        var expected = File.ReadAllText(Path.Combine("Expectation", $"employees.txt")).Standardize();
        Assert.That(ReadOutputStream(), Is.EqualTo(expected));
    }

    [Test]
    [TestCase("-x", ':', ';')]
    [TestCase("--engine-extension=", ':', ';')]
    public async Task Main_AddAndReplaceEngineExtensions_Success(string token, char delimiter, char separator)
    {
        var extensions = new string[] { "txt", "liquid" };
        var engineTags = new string[] { "handlebars", "fluid" };
        using var source = new StreamReader(Path.Combine("data", $"employees.json"));
        Console.SetIn(source);

        var extensionArgs = "";
        for (int i = 0; i < extensions.Length; i++)
            extensionArgs += $"{extensions[i]}{delimiter}{engineTags[i]}{separator}";

        var args = new string[]
        {
            $"-t", $"template/employees.{extensions[0]}",
            $"{token}", extensionArgs,
            $"-r", $"json",
            $"--stdin"
        };
        var exitCode = await Program.Main(args);
        Assert.That(exitCode, Is.EqualTo(0), message: ReadErrorStream());

        var expected = File.ReadAllText(Path.Combine("Expectation", $"employees.txt")).Standardize();
        Assert.That(ReadOutputStream(), Is.EqualTo(expected));
    }

    [Test]
    [TestCase("-X", ':')]
    [TestCase("--parser-extension=", ':')]
    public async Task Main_AddNewParserExtension_Success(string token, char delimiter)
    {
        string extension = "fm";
        string engineTag = "frontmatter";

        var args = new string[]
        {
            $"-t", $"template/employees.hbs",
            $"-s", $"data/employees.{extension}",
            $"{token}", $"{extension}{delimiter}{engineTag}",
        };
        var exitCode = await Program.Main(args);
        Assert.That(exitCode, Is.EqualTo(0), message: ReadErrorStream());

        var expected = File.ReadAllText(Path.Combine("Expectation", $"employees.txt")).Standardize();
        Assert.That(ReadOutputStream(), Is.EqualTo(expected));
    }

    [Test]
    [TestCase("md", "FrontMatter")]
    public async Task Main_ReplaceExistingParserExtension_Success(string extension, string engineTag)
    {
        var args = new string[]
        {
            $"-t", $"template/employees.hbs",
            $"-s", $"data/employees.md",
            $"-X", $"{extension}:{engineTag}",
            $"-r", $"FrontMatter"
        };
        var exitCode = await Program.Main(args);
        Assert.That(exitCode, Is.EqualTo(0), message: ReadErrorStream());

        var expected = File.ReadAllText(Path.Combine("Expectation", $"employees.txt")).Standardize();
        Assert.That(ReadOutputStream(), Is.EqualTo(expected));
    }

    [Test]
    [TestCase("-X", ':', ';')]
    [TestCase("--parser-extension", ':', ';')]
    public async Task Main_AddAndReplaceParserExtensions_Success(string token, char delimiter, char separator)
    {
        var extensions = new string[] { "fm", "dat" };
        var parserTags = new string[] { "FrontMatter", "Json" };

        var extensionArgs = "";
        for (int i = 0; i < extensions.Length; i++)
            extensionArgs += $"{extensions[i]}{delimiter}{parserTags[i]}{separator}";

        var args = new string[]
        {
            $"-t", $"template/employees.hbs",
            $"-s", $"data/employees.{extensions[1]}",
            $"{token}", extensionArgs,
            $"-r", $"json"
        };
        var exitCode = await Program.Main(args);
        Assert.That(exitCode, Is.EqualTo(0), message: ReadErrorStream());

        var expected = File.ReadAllText(Path.Combine("Expectation", $"employees.txt")).Standardize();
        Assert.That(ReadOutputStream(), Is.EqualTo(expected));
    }

    [Test]
    [TestCase("-s", ':', ';')]
    [TestCase("--source", ':', ';')]
    public async Task Main_ManySources_Success(string token, char delimiter, char separator)
    {
        var sources = new KeyValuePair<string, string>[]
        {
            new("organization", "data/organization.json"),
            new("employees", "data/employees.yaml")
        };

        var extensionArgs = "";
        foreach (var source in sources)
            extensionArgs += $"{source.Key}{delimiter}{source.Value}{separator}";

        var args = new string[]
        {
            $"-t", $"template/full_organization.liquid",
            $"{token}", extensionArgs,
        };
        var exitCode = await Program.Main(args);
        Assert.That(exitCode, Is.EqualTo(0), message: ReadErrorStream());

        var expected = File.ReadAllText(Path.Combine("Expectation", $"full_organization.txt")).Standardize();
        Assert.That(ReadOutputStream(), Is.EqualTo(expected));
    }

    [Test]
    public async Task Main_UrlParser_Success(
            [ValueSource(nameof(Templates))] string engine)
    {
        if (engine == "smart")
            Assert.Ignore("SmartFormat is too complex for this rendering.");

        var args = new string[]
        {
            $"-t", $"template/web-address.{engine}",
            $"-s", $"data/web-address.url"
        };
        var exitCode = await Program.Main(args);
        Assert.That(exitCode, Is.EqualTo(0), message: ReadErrorStream());

        var expected = File.ReadAllText(Path.Combine("Expectation", $"web-address.md")).Standardize();
        Assert.That(ReadOutputStream(), Is.EqualTo(expected));
    }
}

public static class StringExtensions
{
    public static string Standardize(this string value)
    {
        var result = value.Trim().Replace("\r\n", "\n").Trim();
        return result;
    }
}
