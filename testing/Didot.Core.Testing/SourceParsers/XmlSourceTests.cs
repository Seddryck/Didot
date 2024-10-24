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
public class XmlSourceTests
{
    [Test]
    public void Parse_SingleProperty_Successful()
    {
        var source = new XmlSource();
        dynamic result = source.Parse("<Root><Name>World</Name></Root>");
        Assert.That(result, Is.AssignableTo<IDictionary<string, object>>());
        Assert.That(result, Does.ContainKey("Name"));
        Assert.That(result["Name"], Is.EqualTo("World"));
    }

    [Test]
    public void Parse_MultipleProperties_Successful()
    {
        var source = new XmlSource();
        dynamic result = source.Parse("<Root><Name>Albert</Name>\r\n<Age>30</Age></Root>");
        Assert.That(result, Is.AssignableTo<IDictionary<string, object>>());
        Assert.That(result, Does.ContainKey("Name"));
        Assert.That(result["Name"], Is.EqualTo("Albert"));
        Assert.That(result, Does.ContainKey("Age"));
        Assert.That(result["Age"], Is.EqualTo("30"));
    }

    [Test]
    public void Parse_NestedProperties_Successful()
    {
        var source = new XmlSource();
        dynamic result = source.Parse("<Root><Name><First>Albert</First>\r\n<Last>Einstein</Last></Name>\r\n<Age>30</Age></Root>");
        Assert.That(result, Is.AssignableTo<IDictionary<string, object>>());
        Assert.That(result, Does.ContainKey("Name"));
        Assert.That(result["Name"], Is.AssignableTo<IDictionary<string, object>>());
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
        var source = new XmlSource();
        dynamic result = source.Parse("<Root><Employee><Name>Albert</Name>\r\n<Age>30</Age></Employee>\r\n<Employee><Name>Nikola</Name>\r\n<Age>50</Age></Employee></Root>");
        Assert.That(result, Is.AssignableTo<List<object>>());
        Assert.That(result[0], Does.ContainKey("Name"));
        Assert.That(result[0]["Name"], Is.EqualTo("Albert"));
        Assert.That(result[0], Does.ContainKey("Age"));
        Assert.That(result[0]["Age"], Is.EqualTo("30"));
    }

    [Test]
    public void Parse_Stream_Successful()
    {
        var source = new XmlSource();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("<Root><Name>World</Name></Root>"));
        dynamic result = source.Parse(stream);
        Assert.That(result, Is.AssignableTo<IDictionary<string, object>>());
        Assert.That(result, Does.ContainKey("Name"));
        Assert.That(result["Name"], Is.EqualTo("World"));
    }
}
