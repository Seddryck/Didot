using System;
using System.Collections.Generic;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Didot.Cli.Testing.RenderOptions;
public class SourceTests
{
    [Test]
    public void Source_Empty_Valid()
    {
        var options = new Cli.RenderOptions();
        var parser = new Parser(new RenderCommand(options));
        var args = new[] { "--template=file1.txt", "--stdin", "--parser=json" };

        var result = parser.Parse(args);
        var context = new InvocationContext(result);

        Assert.That(context.ParseResult.GetValueForOption(options.Sources), Is.Empty);
    }

    [Test]
    [TestCase("-s", "file1.json")]
    [TestCase("-sfile1.json")]
    [TestCase("--source=file1.json")]
    [TestCase("--source", "file1.json")]
    public void Source_One_Valid(params string[] additionalArgs)
    {
        var options = new Cli.RenderOptions();
        var parser = new Parser(new RenderCommand(options));
        var args = new List<string>() { "--template", "file1.txt" };
        args.AddRange(additionalArgs);

        var result = parser.Parse(args);
        var context = new InvocationContext(result);

        Assert.That(context.ParseResult.Errors, Is.Null.Or.Empty);
        var value = context.ParseResult.GetValueForOption(options.Sources);
        Assert.That(value, Is.Not.Null);
        Assert.That(value, Has.Count.EqualTo(1));
        Assert.That(value, Does.ContainKey(string.Empty));
        Assert.That(value[string.Empty], Is.EqualTo("file1.json"));
    }

    [Test]
    [TestCase("-s", "employees:employees.json;organization:org.yaml")]
    [TestCase("--source=employees:employees.json;organization:org.yaml")]
    [TestCase("--source", "employees:employees.json;organization:org.yaml")]
    [TestCase("--source", "employees:employees.json", "organization:org.yaml")]
    [TestCase("--source", "employees:employees.json", "--source", "organization:org.yaml")]
    [TestCase("--source", "employees:employees.json", "-s", "organization:org.yaml")]
    public void Source_Many_Valid(params string[] additionalArgs)
    {
        var options = new Cli.RenderOptions();
        var parser = new Parser(new RenderCommand(options));
        var args = new List<string>() { "--template=file1.txt" };
        args.AddRange(additionalArgs);

        var result = parser.Parse(args);
        var context = new InvocationContext(result);

        Assert.That(context.ParseResult.Errors, Is.Null.Or.Empty);
        var value = context.ParseResult.GetValueForOption(options.Sources);
        Assert.That(value, Is.Not.Null);
        Assert.That(value, Has.Count.EqualTo(2));
        Assert.That(value, Does.ContainKey("employees"));
        Assert.That(value["employees"], Is.EqualTo("employees.json"));
        Assert.That(value, Does.ContainKey("organization"));
        Assert.That(value["organization"], Is.EqualTo("org.yaml"));
    }

    [Test]
    [TestCase("--source", "employees.json", "org.yaml")]
    public void Source_ManyWithoutKeys_Invalid(params string[] additionalArgs)
    {
        var options = new Cli.RenderOptions();
        var parser = new Parser(new RenderCommand(options));
        var args = new List<string>() { "--template=file1.txt", "--stdin", "--parser=json" };
        args.AddRange(additionalArgs);

        var result = parser.Parse(args);
        var context = new InvocationContext(result);

        Assert.That(context.ParseResult.Errors, Is.Not.Null.And.Not.Empty);
        Assert.That(context.ParseResult.Errors.Select(x => x.Message)
            , Does.Contain("The key is missing for the key-value pair: org.yaml. A key is required when multiple key-value pairs are provided."));
    }
}
