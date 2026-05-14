using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Didot.Cli.Testing.RenderOptions;
public class ParserParamsTests
{
    [Test]
    public void ParserParams_Empty_Valid()
    {
        var options = new Cli.RenderOptions();
        var command = new RenderCommand(options);
        var args = new[] { "--template=file1.txt", "--stdin", "--parser=json" };

        var result = command.Parse(args);

        Assert.That(result.GetValue(options.ParserParams), Is.Empty);
    }

    [Test]
    [TestCase("-P:txt@delimiter:Semicolumn")]
    [TestCase("-P=txt@delimiter:Semicolumn")]
    [TestCase("--parser-parameter=txt@delimiter:Semicolumn")]
    public void ParserExtension_One_Valid(string additionalArgs)
    {
        var options = new Cli.RenderOptions();
        var command = new RenderCommand(options);
        var args = new[] { "--template=file1.txt", "--stdin", "--parser=stdin", additionalArgs };

        var result = command.Parse(args);

        Assert.That(result.Errors, Is.Null.Or.Empty);
        var value = result.GetValue(options.ParserParams);
        Assert.That(value, Is.Not.Null);
        Assert.That(value, Has.Count.EqualTo(1));
        Assert.That(value, Does.ContainKey("txt@delimiter"));
        Assert.That(value!["txt@delimiter"], Is.EqualTo("Semicolumn"));
    }

    [Test]
    [TestCase("-P", "txt@delimiter:Semicolumn;txt@commentChar:Dash")]
    [TestCase("-P=txt@delimiter:Semicolumn;txt@commentChar:Dash")]
    [TestCase("--parser-parameter=txt@delimiter:Semicolumn;txt@commentChar:Dash")]
    public void ParserExtension_Many_Valid(params string[] additionalArgs)
    {
        var options = new Cli.RenderOptions();
        var command = new RenderCommand(options);
        var args = (new[] { "--template=file1.txt", "--stdin", "--parser=csv" }).Concat(additionalArgs).ToArray();

        var result = command.Parse(args);

        Assert.That(result.Errors, Is.Null.Or.Empty);
        var value = result.GetValue(options.ParserParams);
        Assert.That(value, Is.Not.Null);
        Assert.That(value, Has.Count.EqualTo(2));
        Assert.That(value, Does.ContainKey("txt@delimiter"));
        Assert.That(value!["txt@delimiter"], Is.Not.Null);
        Assert.That(value["txt@delimiter"], Is.EqualTo("Semicolumn"));
        Assert.That(value, Does.ContainKey("txt@commentChar"));
        Assert.That(value["txt@commentChar"], Is.EqualTo("Dash"));
    }
}
