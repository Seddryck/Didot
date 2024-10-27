using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Didot.Cli.Testing.RenderOptions;
public class TemplateTests
{
    [Test]
    public void Template_Provided_Valid()
    {
        var options = new Cli.RenderOptions();
        var parser = new Parser(new RenderCommand(options));
        var args = new[] { "--template", "file1.txt" };

        var result = parser.Parse(args);
        var context = new InvocationContext(result);

        Assert.That(context.ParseResult.GetValueForOption(options.Template), Is.EqualTo("file1.txt"));
    }

    [Test]
    public void Template_Empty_Valid()
    {
        var options = new Cli.RenderOptions();
        var parser = new Parser(new RenderCommand(options));
        var args = new[] { "--stdin", "--parser=json" };

        var result = parser.Parse(args);
        var context = new InvocationContext(result);

        Assert.That(context.ParseResult.Errors, Is.Not.Null.And.Not.Empty);
        Assert.That(context.ParseResult.Errors.Select(x => x.Message), Does.Contain("Option '-t' is required."));
        Assert.That(context.ParseResult.GetValueForOption(options.Template), Is.Null);
    }
}
