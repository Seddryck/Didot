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
public class FrontmatterSourceTests
{
    [Test]
    public void Parse_SingleProperty_Successful()
    {
        var source = new FrontmatterSource();
        dynamic result = source.Parse("---\r\nName: World\r\n---\r\n## What's new!\r\n\r\nWhere is the world.\r\n");
        Assert.That(result, Is.AssignableTo<IDictionary<object, object>>());
        Assert.That(result, Does.ContainKey("Name"));
        Assert.That(result["Name"], Is.EqualTo("World"));
    }

    [Test]
    public void Parse_MultipleProperties_Successful()
    {
        var source = new FrontmatterSource();
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
        var source = new FrontmatterSource();
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
    public void Parse_Array_Successful()
    {
        var source = new FrontmatterSource();
        dynamic result = source.Parse("---\r\n- Name: Albert\r\n  Age: 30\r\n- Name: Nikola\r\n  Age: 50\r\n---\r\n## What's new!\r\n\r\nWhere is the world.\r\n");
        Assert.That(result, Is.AssignableTo<List<object>>());
        Assert.That(result[0], Does.ContainKey("Name"));
        Assert.That(result[0]["Name"], Is.EqualTo("Albert"));
        Assert.That(result[0], Does.ContainKey("Age"));
        Assert.That(result[0]["Age"], Is.EqualTo("30"));
    }

    [Test]
    public void Parse_Stream_Successful()
    {
        var source = new FrontmatterSource();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("---\r\nName: World\r\n---\r\n## What's new!\r\n\r\nWhere is the world.\r\n"));
        dynamic result = source.Parse(stream);
        Assert.That(result, Is.AssignableTo<IDictionary<object, object>>());
        Assert.That(result, Does.ContainKey("Name"));
        Assert.That(result["Name"], Is.EqualTo("World"));
    }
}
