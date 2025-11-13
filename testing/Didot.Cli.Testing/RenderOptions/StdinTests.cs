using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Didot.Cli;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Didot.Cli.Testing.RenderOptions;
public class StdInTests
{
    [Test]
    [TestCase("-i")]
    [TestCase("--stdin")]
    public void StdIn_Provided_Valid(string stdIn)
    {
        var options = new Cli.RenderOptions();
        var args = new[] { "--template", "file1.txt", "--parser", "json", stdIn };

        var result = new RenderCommand(options).Parse(args);

        Assert.That(result.Errors, Is.Null.Or.Empty);
        Assert.That(result.GetValue(options.StdIn), Is.True);
    }

    [Test]
    public void StdIn_NotProvided_Valid()
    {
        var options = new Cli.RenderOptions();
        var args = new[] { "--template=file1.txt", "--source=file.json" };

        var result = new RenderCommand(options).Parse(args);

        Assert.That(result.Errors, Is.Null.Or.Empty);
        Assert.That(result.GetValue(options.StdIn), Is.False);
    }

    [Test]
    [TestCase(true)]
    public void StdIn_ProvidedExplicitelySet_Valid(bool value)
    {
        var options = new Cli.RenderOptions();
        var args = new[] { "--template", "file1.txt", "--parser", "json", "--stdin", $"{value}" };

        var result = new RenderCommand(options).Parse(args);

        Assert.That(result.Errors, Is.Null.Or.Empty);
        Assert.That(result.GetValue(options.StdIn), Is.EqualTo(value));
    }

    [Test]
    public void StdInSource_BothProvided_Error()
    {
        var options = new Cli.RenderOptions();
        var args = new List<string>() { "--template=file1.txt", "--stdin", "--source=file1.json" };

        var result = new RenderCommand(options).Parse(args);

        Assert.That(result.Errors, Is.Not.Null.And.Not.Empty);
        Assert.That(result.Errors.Select(x => x.Message), Does.Contain("The --stdin option cannot be provided together with the --source option."));
    }

    [Test]
    public void StdInSource_BothProvidedButStdInSetToFalse_Valid()
    {
        var options = new Cli.RenderOptions();
        var args = new List<string>() { "--template", "file1.txt" };
        args.AddRange(new[] { "--stdin", "false" });
        args.AddRange(new[] { "--source", "file1.json" });

        var result = new RenderCommand(options).Parse(args);

        Assert.That(result.Errors, Is.Null.Or.Empty);
    }

    [Test]
    public void StdInParser_ParserNotProvided_Error()
    {
        var options = new Cli.RenderOptions();
        var args = new List<string>() { "--template=file1.txt", "--stdin" };

        var result = new RenderCommand(options).Parse(args);

        Assert.That(result.Errors, Is.Not.Null.And.Not.Empty);
        Assert.That(result.Errors.Select(x => x.Message), Does.Contain("The --parser option is required when using --stdin to specify the input source."));
    }

    [Test]
    public void StdInParser_ParserNotProvidedButStdInSetToFalse_Valid()
    {
        var options = new Cli.RenderOptions();
        var args = new List<string>() { "--template=file1.txt", "--stdin", "false", "--source=file.json" };

        var result = new RenderCommand(options).Parse(args);

        Assert.That(result.Errors, Is.Null.Or.Empty);
    }

    [Test]
    public void StdInParserSource_ParserNotProvidedSourceProvided_Errors()
    {
        var options = new Cli.RenderOptions();
        var args = new List<string>() { "--template", "file1.txt" };
        args.AddRange(new[] { "--stdin" });
        args.AddRange(new[] { "--source", "file1.json" });

        var result = new RenderCommand(options).Parse(args);

        Assert.That(result.Errors, Is.Not.Null.And.Not.Empty);
    }
}
