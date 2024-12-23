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
        Console.SetOut(OriginalError);
    }

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
        Assert.That(exitCode, Is.Not.EqualTo(0));
        Assert.That(ReadErrorStream(), Does.StartWith("Option '-t' is required."));
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
        Assert.That(exitCode, Is.EqualTo(0), message: ReadErrorStream());

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
