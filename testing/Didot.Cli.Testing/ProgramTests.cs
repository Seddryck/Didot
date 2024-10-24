using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.TemplateEngines;
using HandlebarsDotNet;
using NUnit.Framework;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Didot.Cli.Testing;

public class ProgramTests
{
    private static readonly string[] Templates = { "scriban", "liquid", "hbs", "smart", "st" };
    private static readonly string[] DataSets = { "yaml", "json", "xml" };

    private TextWriter OriginalOutput { get; set; }
    private Stream MemoryStream { get; set; }
    private StreamWriter Writer { get; set; }

    [SetUp]
    public void SetUp()
    {
        OriginalOutput = Console.Out;
        MemoryStream = new MemoryStream();
        Writer = new(MemoryStream)
        {
            AutoFlush = true
        };
        Console.SetOut(Writer);
    }

    [TearDown]
    public void TearDown()
    {
        Writer.Dispose();
        MemoryStream.Dispose();
        Console.SetOut(OriginalOutput);
    }

    [Test, Combinatorial]
    public void Main_StdInStdOut_Successful(
            [ValueSource(nameof(Templates))] string engine,
            [ValueSource(nameof(DataSets))] string data)
    {
        using var source = new StreamReader(Path.Combine("data", $"data-01.{data}"));
        Console.SetIn(source);

        var args = new string[]
        {
            $"-ttemplate/template-01.{engine}",
            $"-p{data}"
        };
        Program.Main(args);

        MemoryStream.Position = 0;
        using (var reader = new StreamReader(MemoryStream))
        {
            var consoleOutput = reader.ReadToEnd().Standardize();
            var expected = File.ReadAllText(Path.Combine("Expectation", $"expectation-01.txt")).Standardize();
            Assert.That(consoleOutput, Is.EqualTo(expected));
        }
    }

    [Test, Combinatorial]
    public void Main_SourceFileOutputFile_Successful(
            [ValueSource(nameof(Templates))] string engine,
            [ValueSource(nameof(DataSets))] string data)
    {
        var args = new string[]
        {
            $"-ttemplate/template-01.{engine}",
            $"-sdata/data-01.{data}",
            $"-ooutput-01-{engine}-{data}.txt"
        };
        Program.Main(args);

        var output = File.ReadAllText($"output-01-{engine}-{data}.txt").Standardize();
        var expected = File.ReadAllText(Path.Combine("Expectation", $"expectation-01.txt")).Standardize();
        Assert.That(output, Is.EqualTo(expected));
    }

    [Test]
    public void Main_MissingParserAndSource_Failure()
    {
        var args = new string[]
        {
            $"-ttemplate/template-01.hbs",
            $"-ofailure.txt"
        };
        Program.Main(args);

        MemoryStream.Position = 0;
        using (var reader = new StreamReader(MemoryStream))
        {
            var consoleOutput = reader.ReadToEnd().Standardize();
            Assert.That(consoleOutput, Does.StartWith("Error: Missing input source."));
        }
    }

    [Test]
    public void Main_MissingTemplate_Failure()
    {
        var args = new string[]
        {
            $"-pjson",
            $"-ofailure.txt"
        };
        Program.Main(args);

        MemoryStream.Position = 0;
        using (var reader = new StreamReader(MemoryStream))
        {
            var consoleOutput = reader.ReadToEnd().Standardize();
            Assert.That(consoleOutput, Does.StartWith("Error parsing arguments."));
        }
    }

    [Test]
    public void Main_ForcedEngine_Success()
    {
        var args = new string[]
        {
            $"-ttemplate/template-01.liquid",
            $"-sdata/data-01.json",
            $"-efluid"
        };
        Program.Main(args);

        MemoryStream.Position = 0;
        using (var reader = new StreamReader(MemoryStream))
        {
            var consoleOutput = reader.ReadToEnd().Standardize();
            var expected = File.ReadAllText(Path.Combine("Expectation", $"expectation-01.txt")).Standardize();
            Assert.That(consoleOutput, Is.EqualTo(expected));
        }
    }

    [Test]
    [TestCase("-x", ':')]
    [TestCase("-x ", ':')]
    [TestCase("--EngineExtension=", ':')]
    public void Main_AddNewEngineExtension_Success(string token, char delimiter)
    {
        string extension = "txt";
        string engineTag = "handlebars";
        using var source = new StreamReader(Path.Combine("data", $"data-01.json"));
        Console.SetIn(source);

        var args = new string[]
        {
            $"-ttemplate/template-01.{extension}",
            $"{token}{extension}{delimiter}{engineTag}",
            $"-pjson"
        };
        Program.Main(args);

        MemoryStream.Position = 0;
        using (var reader = new StreamReader(MemoryStream))
        {
            var consoleOutput = reader.ReadToEnd().Standardize();
            var expected = File.ReadAllText(Path.Combine("Expectation", $"expectation-01.txt")).Standardize();
            Assert.That(consoleOutput, Is.EqualTo(expected));
        }
    }

    [Test]
    [TestCase("liquid", "fluid")]
    public void Main_ReplaceExistingEngineExtension_Success(string extension, string engineTag)
    {
        using var source = new StreamReader(Path.Combine("data", $"data-01.json"));
        Console.SetIn(source);

        var args = new string[]
        {
            $"-ttemplate/template-01.{extension}",
            $"-x{extension}:{engineTag}",
            $"-pjson"
        };
        Program.Main(args);

        MemoryStream.Position = 0;
        using (var reader = new StreamReader(MemoryStream))
        {
            var consoleOutput = reader.ReadToEnd().Standardize();
            var expected = File.ReadAllText(Path.Combine("Expectation", $"expectation-01.txt")).Standardize();
            Assert.That(consoleOutput, Is.EqualTo(expected));
        }
    }

    [Test]
    [TestCase("-x", ':', ';')]
    [TestCase("-x ", ':', ';')]
    [TestCase("--EngineExtension=", ':', ';')]
    public void Main_AddAndReplaceEngineExtensions_Success(string token, char delimiter, char separator)
    {
        var extensions = new string[] { "txt", "liquid" };
        var engineTags = new string[] { "handlebars", "fluid" };
        using var source = new StreamReader(Path.Combine("data", $"data-01.json"));
        Console.SetIn(source);

        var extensionArgs = $"{token}";
        for (int i = 0; i < extensions.Length; i++)
            extensionArgs += $"{extensions[i]}{delimiter}{engineTags[i]}{separator}";

        var args = new string[]
        {
            $"-ttemplate/template-01.{extensions[0]}",
            extensionArgs,
            $"-pjson"
        };
        Program.Main(args);

        MemoryStream.Position = 0;
        using (var reader = new StreamReader(MemoryStream))
        {
            var consoleOutput = reader.ReadToEnd().Standardize();
            var expected = File.ReadAllText(Path.Combine("Expectation", $"expectation-01.txt")).Standardize();
            Assert.That(consoleOutput, Is.EqualTo(expected));
        }
    }

    [Test]
    [TestCase("-X", ':')]
    [TestCase("-X ", ':')]
    [TestCase("--ParserExtension=", ':')]
    public void Main_AddNewParserExtension_Success(string token, char delimiter)
    {
        string extension = "fm";
        string engineTag = "frontmatter";

        var args = new string[]
        {
            $"-ttemplate/template-01.hbs",
            $"-sdata/data-01.{extension}",
            $"{token}{extension}{delimiter}{engineTag}",
        };
        Program.Main(args);

        MemoryStream.Position = 0;
        using (var reader = new StreamReader(MemoryStream))
        {
            var consoleOutput = reader.ReadToEnd().Standardize();
            var expected = File.ReadAllText(Path.Combine("Expectation", $"expectation-01.txt")).Standardize();
            Assert.That(consoleOutput, Is.EqualTo(expected));
        }
    }

    [Test]
    [TestCase("md", "FrontMatter")]
    public void Main_ReplaceExistingParserExtension_Success(string extension, string engineTag)
    {
        var args = new string[]
        {
            $"-ttemplate/template-01.hbs",
            $"-sdata/data-01.md",
            $"-X{extension}:{engineTag}",
            $"-pFrontMatter"
        };
        Program.Main(args);

        MemoryStream.Position = 0;
        using (var reader = new StreamReader(MemoryStream))
        {
            var consoleOutput = reader.ReadToEnd().Standardize();
            var expected = File.ReadAllText(Path.Combine("Expectation", $"expectation-01.txt")).Standardize();
            Assert.That(consoleOutput, Is.EqualTo(expected));
        }
    }

    [Test]
    [TestCase("-X", ':', ';')]
    [TestCase("-X ", ':', ';')]
    [TestCase("--ParserExtension=", ':', ';')]
    public void Main_AddAndReplaceParserExtensions_Success(string token, char delimiter, char separator)
    {
        var extensions = new string[] { "fm", "dat" };
        var parserTags = new string[] { "FrontMatter", "Json" };

        var extensionArgs = $"{token}";
        for (int i = 0; i < extensions.Length; i++)
            extensionArgs += $"{extensions[i]}{delimiter}{parserTags[i]}{separator}";

        var args = new string[]
        {
            $"-ttemplate/template-01.hbs",
            $"-sdata/data-01.{extensions[1]}",
            extensionArgs,
            $"-pjson"
        };
        Program.Main(args);

        MemoryStream.Position = 0;
        using (var reader = new StreamReader(MemoryStream))
        {
            var consoleOutput = reader.ReadToEnd().Standardize();
            var expected = File.ReadAllText(Path.Combine("Expectation", $"expectation-01.txt")).Standardize();
            Assert.That(consoleOutput, Is.EqualTo(expected));
        }
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
