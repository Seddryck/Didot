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
    public void CaptureConsoleOutputTest(
            [ValueSource(nameof(Templates))] string engine,
            [ValueSource(nameof(DataSets))] string data)
    {
        var args = new string[]
        {
                    $"-ttemplate/template-01.{engine}",
                    $"-sdata/data-01.{data}"
        };
        Program.Main(args);

        MemoryStream.Position = 0;
        using (var reader = new StreamReader(MemoryStream))
        {
            var consoleOutput = reader.ReadToEnd().Standardize();
            var expected = File.ReadAllText($"Expectation/expectation-01.txt").Standardize();
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
