using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Didot.Cli.Testing;

public class ProgramTests
{
    private static readonly string[] Templates = { "scriban", "liquid", "hbs", "smart" };
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
}

public static class StringExtensions
{
    public static string Standardize(this string value)
    {
        var result = value.Trim().Replace("\r\n", "\n").Trim();
        return result;
    }
}
