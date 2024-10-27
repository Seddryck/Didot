using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Didot.Cli.Testing.RenderOptions;
public class OutputTests
{
    [Test]
    [TestCase("-o", "file1.txt")]
    [TestCase("--output", "file1.txt")]
    public void Output_Provided_Valid(params string[] optionArray)
    {
        var options = new Cli.RenderOptions();
        var parser = new Parser(new RenderCommand(options));
        var args = new List<string>() { "--template=file1.txt", "--stdin", "--parser=json" };
        args.AddRange(optionArray);

        var result = parser.Parse(args);
        var context = new InvocationContext(result);

        Assert.That(context.ParseResult.Errors, Is.Null.Or.Empty);
        Assert.That(context.ParseResult.GetValueForOption(options.Output), Is.EqualTo("file1.txt"));
    }

    [Test]
    public void Output_NotProvided_Valid()
    {
        var options = new Cli.RenderOptions();
        var parser = new Parser(new RenderCommand(options));
        var args = new[] { "--template=file1.txt", "--stdin", "--parser=json" };

        var result = parser.Parse(args);
        var context = new InvocationContext(result);

        Assert.That(context.ParseResult.Errors, Is.Null.Or.Empty);
        Assert.That(context.ParseResult.GetValueForOption(options.Output), Is.Null);
    }
}
