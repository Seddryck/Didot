using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Didot.Cli.Testing.RenderOptions;
public class ParserExtensionTests
{
    [Test]
    public void ParserExtension_Empty_Valid()
    {
        var options = new Cli.RenderOptions();
        var parser = new Parser(new RenderCommand(options));
        var args = new List<string>() { "--template=file1.txt", "--stdin", "--parser=json" };

        var result = parser.Parse(args);
        var context = new InvocationContext(result);

        Assert.That(context.ParseResult.GetValueForOption(options.ParserExtensions), Is.Empty);
    }

    [Test]
    [TestCase("-X .txt:json")]
    [TestCase("-X.txt:json")]
    [TestCase("--parser-extension=.txt:json")]
    public void ParserExtension_One_Valid(string additionalArgs)
    {
        var options = new Cli.RenderOptions();
        var parser = new Parser(new RenderCommand(options));
        var args = new List<string>() { "--template=file1.txt", "--stdin", "--parser=json", additionalArgs };

        var result = parser.Parse(args);
        var context = new InvocationContext(result);

        Assert.That(context.ParseResult.Errors, Is.Null.Or.Empty);
        var value = context.ParseResult.GetValueForOption(options.ParserExtensions);
        Assert.That(value, Is.Not.Null);
        Assert.That(value, Has.Count.EqualTo(1));
        Assert.That(value, Does.ContainKey(".txt"));
        Assert.That(value[".txt"], Is.EqualTo("json"));
    }

    [Test]
    [TestCase("-X", ".txt:json;.y:yaml")]
    [TestCase("-X.txt:json;.y:yaml")]
    [TestCase("--parser-extension=.txt:json;.y:yaml")]
    [TestCase("--parser-extension", ".txt:json", ".y:yaml")]
    [TestCase("--parser-extension", ".txt:json", "--parser-extension", ".y:yaml")]
    public void ParserExtension_Many_Valid(params string[] additionalArgs)
    {
        var options = new Cli.RenderOptions();
        var parser = new Parser(new RenderCommand(options));
        var args = new List<string>() { "--template=file1.txt", "--stdin", "--parser=json" };
        args.AddRange(additionalArgs);

        var result = parser.Parse(args);
        var context = new InvocationContext(result);

        Assert.That(context.ParseResult.Errors, Is.Null.Or.Empty);
        var value = context.ParseResult.GetValueForOption(options.ParserExtensions);
        Assert.That(value, Is.Not.Null);
        Assert.That(value, Has.Count.EqualTo(2));
        Assert.That(value, Does.ContainKey(".txt"));
        Assert.That(value[".txt"], Is.EqualTo("json"));
        Assert.That(value, Does.ContainKey(".y"));
        Assert.That(value[".y"], Is.EqualTo("yaml"));
    }
}
