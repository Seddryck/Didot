using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Didot.Cli.Testing;

public class ProgramTests
{
    private static string[] Templates = { "scriban", "liquid", "hbs", "smart" };
    public static string[] DataSets = { "yaml", "json", "xml" };

    [Test, Combinatorial]
    public void Main_WithNoArguments_ShouldPrintHelp(
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

        var output = File.ReadAllText($"output-01-{engine}-{data}.txt").Uniform();
        var expected = File.ReadAllText($"Expectation/expectation-01.txt").Uniform();
        Assert.That(output, Is.EqualTo(expected));
    }
}

public static class StringExtensions
{
    public static string Uniform(this string value)
    {
        var result = value.Trim().Replace("\r\n", "\n").Trim();
        return result;
    }
}
