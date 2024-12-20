using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.SourceParsers;
using NUnit.Framework;
using PocketCsvReader;

namespace Didot.Core.Testing.SourceParsers;
public class CsvSourceBuilderTests
{
    [Test]
    [TestCase(';')]
    [TestCase(',')]
    [TestCase('\t')]
    public void Build_SingleParameter_Successful(char delimiter)
    {
        var builder = new CsvSourceBuilder();
        var parameters = new Dictionary<string, string>()
        {
            { "csv@delimiter", delimiter.ToString() }
        };
        var parser = builder.Build(parameters, ".csv");
        Assert.That(parser, Is.AssignableTo<CsvSource>());
        var dialect = ((CsvSource)parser).Dialect;
        Assert.That(dialect.Delimiter, Is.EqualTo(delimiter));
    }

    [Test]
    [TestCase(';')]
    [TestCase('\t')]
    public void Build_SingleParameterNotCorrectExtension_Successful(char delimiter)
    {
        var builder = new CsvSourceBuilder();
        var parameters = new Dictionary<string, string>()
        {
            { "data@delimiter", delimiter.ToString() }
        };
        var parser = builder.Build(parameters, ".csv");
        Assert.That(parser, Is.AssignableTo<CsvSource>());
        var dialect = ((CsvSource)parser).Dialect;
        Assert.That(dialect.Delimiter, Is.EqualTo(','));
    }

    [Test]
    [TestCase("delimiter", ';')]
    [TestCase("quoteChar", '"')]
    [TestCase("escapeChar", '\\')]
    [TestCase("commentChar", '#')]
    public void Build_SingleParameterChar_Successful(string parameter, char delimiter)
    {
        var property = typeof(CsvDialectDescriptor).GetProperty(parameter,
                            System.Reflection.BindingFlags.Public
                            | System.Reflection.BindingFlags.Instance
                            | System.Reflection.BindingFlags.IgnoreCase)!;

        var builder = new CsvSourceBuilder();
        var parameters = new Dictionary<string, string>()
        {
            { $"csv@{parameter}", delimiter.ToString() }
        };
        var parser = builder.Build(parameters, ".csv");
        Assert.That(parser, Is.AssignableTo<CsvSource>());
        var dialect = ((CsvSource)parser).Dialect;
        Assert.That(property.GetValue(dialect)!, Is.EqualTo(delimiter));
    }

    [Test]
    [TestCase("delimiter", ';', "Semicolon")]
    [TestCase("quoteChar", '"', "DoubleQuote")]
    [TestCase("escapeChar", '\\', "BackSlash")]
    [TestCase("commentChar", '#', "Hash")]
    public void Build_SingleParameterCharSynonym_Successful(string parameter, char delimiter, string alias)
    {
        var property = typeof(CsvDialectDescriptor).GetProperty(parameter,
                            System.Reflection.BindingFlags.Public
                            | System.Reflection.BindingFlags.Instance
                            | System.Reflection.BindingFlags.IgnoreCase)!;

        var builder = new CsvSourceBuilder();
        var parameters = new Dictionary<string, string>()
        {
            { $"csv@{parameter}", alias }
        };
        var parser = builder.Build(parameters, ".csv");
        Assert.That(parser, Is.AssignableTo<CsvSource>());
        var dialect = ((CsvSource)parser).Dialect;
        Assert.That(property.GetValue(dialect)!, Is.EqualTo(delimiter));
    }

    [Test]
    [TestCase("lineTerminator", "\r\n")]
    public void Build_SingleParameterString_Successful(string parameter, string value)
    {
        var property = typeof(CsvDialectDescriptor).GetProperty(parameter,
                            System.Reflection.BindingFlags.Public
                            | System.Reflection.BindingFlags.Instance
                            | System.Reflection.BindingFlags.IgnoreCase)!;

        var builder = new CsvSourceBuilder();
        var parameters = new Dictionary<string, string>()
        {
            { $"csv@{parameter}", value }
        };
        var parser = builder.Build(parameters, ".csv");
        Assert.That(parser, Is.AssignableTo<CsvSource>());
        var dialect = ((CsvSource)parser).Dialect;
        Assert.That(property.GetValue(dialect)!, Is.EqualTo(value));
    }

    [Test]
    [TestCase("lineTerminator", "\r\n", "CarriageReturnLineFeed")]
    [TestCase("lineTerminator", "\r", "CarriageReturn")]
    [TestCase("lineTerminator", "\n", "LineFeed")]
    public void Build_SingleParameterStringSynonym_Successful(string parameter, string value, string alias)
    {
        var property = typeof(CsvDialectDescriptor).GetProperty(parameter,
                            System.Reflection.BindingFlags.Public
                            | System.Reflection.BindingFlags.Instance
                            | System.Reflection.BindingFlags.IgnoreCase)!;

        var builder = new CsvSourceBuilder();
        var parameters = new Dictionary<string, string>()
        {
            { $"csv@{parameter}", alias }
        };
        var parser = builder.Build(parameters, ".csv");
        Assert.That(parser, Is.AssignableTo<CsvSource>());
        var dialect = ((CsvSource)parser).Dialect;
        Assert.That(property.GetValue(dialect)!, Is.EqualTo(value));
    }

    [Test]
    [TestCase("doubleQuote", true)]
    [TestCase("skipInitialSpace", true)]
    [TestCase("header", true)]
    public void Build_SingleParameterBoolean_Successful(string parameter, bool value)
    {
        var property = typeof(CsvDialectDescriptor).GetProperty(parameter,
                            System.Reflection.BindingFlags.Public
                            | System.Reflection.BindingFlags.Instance
                            | System.Reflection.BindingFlags.IgnoreCase)!;

        var builder = new CsvSourceBuilder();
        var parameters = new Dictionary<string, string>()
        {
            { $"csv@{parameter}", value.ToString() }
        };
        var parser = builder.Build(parameters, ".csv");
        Assert.That(parser, Is.AssignableTo<CsvSource>());
        var dialect = ((CsvSource)parser).Dialect;
        Assert.That(property.GetValue(dialect)!, Is.EqualTo(value));
    }

    [Test]
    [TestCase("doubleQuote", true)]
    [TestCase("skipInitialSpace", true)]
    [TestCase("header", true)]
    public void Build_SingleParameterBooleanNumeric_Successful(string parameter, bool value)
    {
        var property = typeof(CsvDialectDescriptor).GetProperty(parameter,
                            System.Reflection.BindingFlags.Public
                            | System.Reflection.BindingFlags.Instance
                            | System.Reflection.BindingFlags.IgnoreCase)!;

        var builder = new CsvSourceBuilder();
        var parameters = new Dictionary<string, string>()
        {
            { $"csv@{parameter}", Convert.ToInt32(value).ToString() }
        };
        var parser = builder.Build(parameters, ".csv");
        Assert.That(parser, Is.AssignableTo<CsvSource>());
        var dialect = ((CsvSource)parser).Dialect;
        Assert.That(property.GetValue(dialect)!, Is.EqualTo(value));
    }
}
