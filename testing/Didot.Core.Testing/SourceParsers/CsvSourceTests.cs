using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.SourceParsers;
using NUnit.Framework;

namespace Didot.Core.Testing.SourceParsers;
public class CsvSourceTests
{
    [Test]
    public void Parse_SingleProperty_Successful()
    {
        var source = new CsvSource();
        dynamic result = source.Parse("Name\r\nWorld");
        Assert.That(result, Is.AssignableTo<IDictionary<string, object>>());
        Assert.That(result, Does.ContainKey("Name"));
        Assert.That(result["Name"], Is.EqualTo("World"));
    }

    [Test]
    public void Parse_MultipleProperties_Successful()
    {
        var source = new CsvSource();
        dynamic result = source.Parse("Name,Age\r\nAlbert,30");
        Assert.That(result, Is.AssignableTo<IDictionary<string, object>>());
        Assert.That(result, Does.ContainKey("Name"));
        Assert.That(result["Name"], Is.EqualTo("Albert"));
        Assert.That(result, Does.ContainKey("Age"));
        Assert.That(result["Age"], Is.EqualTo("30"));
    }

    [Test]
    public void Parse_NestedProperties_Successful()
    {
        var source = new CsvSource();
        dynamic result = source.Parse("Name.First,Name.Last,Age\r\nAlbert,Einstein,30\r\nNikola,Tesla,50");
        Assert.That(result, Is.AssignableTo<List<object>>());
        Assert.That(result[0], Does.ContainKey("Name"));
        Assert.That(result[0]["Name"], Is.AssignableTo<IDictionary<string, object>>());
        Assert.That(result[0]["Name"], Does.ContainKey("First"));
        Assert.That(result[0]["Name"]["First"], Is.EqualTo("Albert"));
        Assert.That(result[0]["Name"], Does.ContainKey("Last"));
        Assert.That(result[0]["Name"]["Last"], Is.EqualTo("Einstein"));
        Assert.That(result[0], Does.ContainKey("Age"));
        Assert.That(result[0]["Age"], Is.EqualTo("30"));
    }

    [Test]
    public void Parse_Array_Successful()
    {
        var source = new CsvSource();
        dynamic result = source.Parse("Name,Age\r\nAlbert,30\r\nNikola,50");
        Assert.That(result, Is.AssignableTo<List<object>>());
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result[0], Does.ContainKey("Name"));
        Assert.That(result[0]["Name"], Is.EqualTo("Albert"));
        Assert.That(result[0], Does.ContainKey("Age"));
        Assert.That(result[0]["Age"], Is.EqualTo("30"));
        Assert.That(result[1], Does.ContainKey("Name"));
        Assert.That(result[1]["Name"], Is.EqualTo("Nikola"));
        Assert.That(result[1], Does.ContainKey("Age"));
        Assert.That(result[1]["Age"], Is.EqualTo("50"));
    }

    [Test]
    public void Parse_Stream_Successful()
    {
        var source = new CsvSource();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("Name\r\nWorld"));
        dynamic result = source.Parse(stream);
        Assert.That(result, Is.AssignableTo<IDictionary<string, object>>());
        Assert.That(result, Does.ContainKey("Name"));
        Assert.That(result["Name"], Is.EqualTo("World"));
    }
}
