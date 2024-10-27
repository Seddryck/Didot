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
        var parser = new Parser(new RenderCommand(options));
        var args = new[] { "--template=file1.txt", "--stdin", option };

        var result = parser.Parse(args);
        var context = new InvocationContext(result);

        Assert.That(context.ParseResult.Errors, Is.Null.Or.Empty);
        Assert.That(context.ParseResult.GetValueForOption(options.Parser), Is.EqualTo("json"));
    }

    [Test]
    public void Parser_NotProvided_Valid()
    {
        var options = new Cli.RenderOptions();
        var parser = new Parser(new RenderCommand(options));
        var args = new[] { "--template=file1.txt", "--source=file.txt" };

        var result = parser.Parse(args);
        var context = new InvocationContext(result);

        Assert.That(context.ParseResult.Errors, Is.Null.Or.Empty);
        Assert.That(context.ParseResult.GetValueForOption(options.Parser), Is.Null);
    }
}
