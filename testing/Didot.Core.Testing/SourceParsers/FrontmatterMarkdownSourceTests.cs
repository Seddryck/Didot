using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.SourceParsers;
using NUnit.Framework;
using SmartFormat.Core.Extensions;

namespace Didot.Core.Testing.SourceParsers;
public class FrontmatterMarkdownSourceTests
{
    [Test]
    public void Parse_SingleProperty_Successful()
    {
        var source = new FrontmatterMarkdownSource();
        dynamic result = source.Parse("---\r\nName: World\r\n---\r\n## What's new!\r\n\r\nWhere is the world.\r\n");
        Assert.That(result, Is.AssignableTo<IDictionary<object, object>>());
        Assert.That(result, Does.ContainKey("Name"));
        Assert.That(result["Name"], Is.EqualTo("World"));
    }

    [Test]
    public void Parse_Content_Successful()
    {
        var source = new FrontmatterMarkdownSource();
        dynamic result = source.Parse("---\r\nName: World\r\n---\r\n## What's new!\r\n\r\nWhere is the world.\r\n");
        Assert.That(result, Is.AssignableTo<IDictionary<object, object>>());
        Assert.That(result, Does.ContainKey("Content"));
        Assert.That(result["Content"], Is.EqualTo("## What's new!\r\n\r\nWhere is the world.\r\n"));
    }

    [Test]
    public void Parse_MultipleProperties_Successful()
    {
        var source = new FrontmatterMarkdownSource();
        dynamic result = source.Parse("---\r\nName: Albert\r\nAge: 30\r\n---\r\n## What's new!\r\n\r\nWhere is the world.\r\n");
        Assert.That(result, Is.AssignableTo<IDictionary<object, object>>());
        Assert.That(result, Does.ContainKey("Name"));
        Assert.That(result["Name"], Is.EqualTo("Albert"));
        Assert.That(result, Does.ContainKey("Age"));
        Assert.That(result["Age"], Is.EqualTo("30"));
    }

    [Test]
    public void Parse_NestedProperties_Successful()
    {
        var source = new FrontmatterMarkdownSource();
        dynamic result = source.Parse("---\r\nName:\r\n  First: Albert\r\n  Last: Einstein\r\nAge: 30\r\n---\r\n## What's new!\r\n\r\nWhere is the world.\r\n");
        Assert.That(result, Is.AssignableTo<IDictionary<object, object>>());
        Assert.That(result, Does.ContainKey("Name"));
        Assert.That(result["Name"], Is.AssignableTo<IDictionary<object, object>>());
        Assert.That(result["Name"], Does.ContainKey("First"));
        Assert.That(result["Name"]["First"], Is.EqualTo("Albert"));
        Assert.That(result["Name"], Does.ContainKey("Last"));
        Assert.That(result["Name"]["Last"], Is.EqualTo("Einstein"));
        Assert.That(result, Does.ContainKey("Age"));
        Assert.That(result["Age"], Is.EqualTo("30"));
    }

    [Test]
    public void Parse_Stream_Successful()
    {
        var source = new FrontmatterMarkdownSource();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("---\r\nName: World\r\n---\r\n## What's new!\r\n\r\nWhere is the world.\r\n"));
        dynamic result = source.Parse(stream);
        Assert.That(result, Is.AssignableTo<IDictionary<object, object>>());
        Assert.That(result, Does.ContainKey("Name"));
        Assert.That(result["Name"], Is.EqualTo("World"));
    }
}
