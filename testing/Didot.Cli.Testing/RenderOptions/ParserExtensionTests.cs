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
        var command = new RenderCommand(options);
        var args = new[] { "--template=file1.txt", "--stdin", "--parser=json" };

        var result = command.Parse(args);

        Assert.That(result.GetValue(options.ParserExtensions), Is.Empty);
    }

    [Test]
    [TestCase("-X .txt:json")]
    [TestCase("-X.txt:json")]
    [TestCase("--parser-extension=.txt:json")]
    public void ParserExtension_One_Valid(string additionalArgs)
    {
        var options = new Cli.RenderOptions();
        var command = new RenderCommand(options);
        var baseArgs = new[] { "--template=file1.txt", "--stdin", "--parser=json" };
        var splitAdditionalArgs = additionalArgs.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var args = baseArgs.Concat(splitAdditionalArgs).ToArray();

        var result = command.Parse(args);

        Assert.That(result.Errors, Is.Null.Or.Empty);
        var value = result.GetValue(options.ParserExtensions);
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
        var command = new RenderCommand(options);
        var args = (new[] { "--template=file1.txt", "--stdin", "--parser=json" })
            .Concat(additionalArgs)
            .ToArray();

        var result = command.Parse(args);


        Assert.That(result.Errors, Is.Null.Or.Empty);
        var value = result.GetValue(options.ParserExtensions);
        Assert.That(value, Is.Not.Null);
        Assert.That(value, Has.Count.EqualTo(2));
        Assert.That(value, Does.ContainKey(".txt"));
        Assert.That(value![".txt"], Is.EqualTo("json"));
        Assert.That(value, Does.ContainKey(".y"));
        Assert.That(value[".y"], Is.EqualTo("yaml"));
    }
}
