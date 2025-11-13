using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Didot.Cli.Testing.RenderOptions;
public class EngineExtensionTests
{
    [Test]
    public void EngineExtension_Empty_Valid()
    {
        var options = new Cli.RenderOptions();
        var args = new List<string>() { "--template=file1.txt", "--stdin", "--parser=json" };

        var result = new RenderCommand(options).Parse(args);

        Assert.That(result.GetValue(options.EngineExtensions), Is.Empty);
    }

    [Test]
    [TestCase("-x .liquid:fluid")]
    [TestCase("-x.liquid:fluid")]
    [TestCase("--engine-extension=.liquid:fluid")]
    public void EngineExtension_One_Valid(string additionalArgs)
    {
        var options = new Cli.RenderOptions();
        var args = new List<string>() { "--template=file1.txt", "--stdin", "--parser=json", additionalArgs };

        var result = new RenderCommand(options).Parse(args);

        Assert.That(result.Errors, Is.Null.Or.Empty);
        var value = result.GetValue(options.EngineExtensions);
        Assert.That(value, Is.Not.Null);
        Assert.That(value, Has.Count.EqualTo(1));
        Assert.That(value, Does.ContainKey(".liquid"));
        Assert.That(value[".liquid"], Is.EqualTo("fluid"));
    }

    [Test]
    [TestCase("-x .liquid:fluid;.txt:handlebars")]
    [TestCase("-x.liquid:fluid;.txt:handlebars")]
    [TestCase("--engine-extension=.liquid:fluid;.txt:handlebars")]
    public void EngineExtension_Many_Valid(string additionalArgs)
    {
        var options = new Cli.RenderOptions();
        var args = new List<string>() { "--template=file1.txt", "--stdin", "--parser=json", additionalArgs };

        var result = new RenderCommand(options).Parse(args);

        Assert.That(result.Errors, Is.Null.Or.Empty);
        var value = result.GetValue(options.EngineExtensions);
        Assert.That(value, Is.Not.Null);
        Assert.That(value, Has.Count.EqualTo(2));
        Assert.That(value, Does.ContainKey(".liquid"));
        Assert.That(value[".liquid"], Is.EqualTo("fluid"));
        Assert.That(value, Does.ContainKey(".txt"));
        Assert.That(value[".txt"], Is.EqualTo("handlebars"));
    }
}
