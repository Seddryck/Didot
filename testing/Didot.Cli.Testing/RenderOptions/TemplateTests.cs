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
        var args = new[] { "--template", "file1.txt" };

        var result = new RenderCommand(options).Parse(args);

        Assert.That(result.GetValue(options.Template), Is.EqualTo("file1.txt"));
    }

    [Test]
    public void Template_Empty_Valid()
    {
        var options = new Cli.RenderOptions();
        var args = new[] { "--stdin", "--parser=json" };

        var result = new RenderCommand(options).Parse(args);

        Assert.That(result.Errors, Is.Not.Null.And.Not.Empty);
        Assert.That(result.Errors.Select(x => x.Message), Does.Contain("Option '--template' is required."));
    }
}
