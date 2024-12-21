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
        var parser = new Parser(new RenderCommand(options));
        var args = new List<string>() { "--template=file1.txt", "--stdin", "--parser=json" };

        var result = parser.Parse(args);
        var context = new InvocationContext(result);

        Assert.That(context.ParseResult.GetValueForOption(options.ParserParams), Is.Empty);
    }

    [Test]
    [TestCase("-P:txt@delimiter:Semicolumn")]
    [TestCase("-P=txt@delimiter:Semicolumn")]
    [TestCase("--parser-parameter=txt@delimiter:Semicolumn")]
    public void ParserExtension_One_Valid(string additionalArgs)
    {
        var options = new Cli.RenderOptions();
        var parser = new Parser(new RenderCommand(options));
        var args = new List<string>() { "--template=file1.txt", "--stdin", "--parser=stdin", additionalArgs };

        var result = parser.Parse(args);
        var context = new InvocationContext(result);

        Assert.That(context.ParseResult.Errors, Is.Null.Or.Empty);
        var value = context.ParseResult.GetValueForOption(options.ParserParams);
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
        var parser = new Parser(new RenderCommand(options));
        var args = new List<string>() { "--template=file1.txt", "--stdin", "--parser=csv" };
        args.AddRange(additionalArgs);

        var result = parser.Parse(args);
        var context = new InvocationContext(result);

        Assert.That(context.ParseResult.Errors, Is.Null.Or.Empty);
        var value = context.ParseResult.GetValueForOption(options.ParserParams);
        Assert.That(value, Is.Not.Null);
        Assert.That(value, Has.Count.EqualTo(2));
        Assert.That(value, Does.ContainKey("txt@delimiter"));
        Assert.That(value!["txt@delimiter"], Is.Not.Null);
        Assert.That(value["txt@delimiter"], Is.EqualTo("Semicolumn"));
        Assert.That(value, Does.ContainKey("txt@commentChar"));
        Assert.That(value["txt@commentChar"], Is.EqualTo("Dash"));
    }
}
