using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Didot.Cli.Testing.RenderOptions;
public class ParserTests
{
    [Test]
    [TestCase("-rjson")]
    [TestCase("-r json")]
    [TestCase("--parser=json")]
    public void Parser_Provided_Valid(string option)
    {
        var options = new Cli.RenderOptions();
        var args = new[] { "--template=file1.txt", "--stdin", option };

        var result = new RenderCommand(options).Parse(args);

        Assert.That(result.Errors, Is.Null.Or.Empty);
        Assert.That(result.GetValue(options.Parser), Is.EqualTo("json"));
    }

    [Test]
    public void Parser_NotProvided_Valid()
    {
        var options = new Cli.RenderOptions();
        var args = new[] { "--template=file1.txt", "--source=file.txt" };

        var result = new RenderCommand(options).Parse(args);

        Assert.That(result.Errors, Is.Null.Or.Empty);
        Assert.That(result.GetValue(options.Parser), Is.Null);
    }
}
