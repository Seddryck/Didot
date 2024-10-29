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
public class UrlSourceTests
{
    [Test]
    [TestCase("https")]
    [TestCase("http")]
    [TestCase("ftp")]
    public void Parse_Scheme_Successful(string scheme)
    {
        var source = new UrlSource();
        dynamic result = source.Parse($"{scheme}://www.domain.org");
        Assert.That(result, Is.AssignableTo<IDictionary<string, object>>());
        Assert.That(result, Does.ContainKey("Scheme"));
        Assert.That(result["Scheme"], Is.EqualTo(scheme));
    }

    [Test]
    [TestCase("https://www.domain.com")]
    [TestCase("https://www.domain.com/page.html")]
    [TestCase("https://www.domain.com/path/page.html")]
    public void Parse_HostName_Successful(string url)
    {
        var source = new UrlSource();
        dynamic result = source.Parse(url);
        Assert.That(result, Is.AssignableTo<IDictionary<string, object>>());
        Assert.That(result, Does.ContainKey("Hostname"));
        Assert.That(result["Hostname"], Is.EqualTo("www.domain.com"));
    }

    [Test]
    [TestCase("https://domain.com")]
    [TestCase("https://domain.com/page.html")]
    [TestCase("https://domain.com/path/page.html")]
    public void Parse_HostNameShort_Successful(string url)
    {
        var source = new UrlSource();
        dynamic result = source.Parse(url);
        Assert.That(result, Is.AssignableTo<IDictionary<string, object>>());
        Assert.That(result, Does.ContainKey("Hostname"));
        Assert.That(result["Hostname"], Is.EqualTo("domain.com"));
    }

    [Test]
    [TestCase("https://127.0.0.1")]
    [TestCase("https://127.0.0.1/page.html")]
    [TestCase("https://127.0.0.1/path/page.html")]
    public void Parse_HostNameAsIP_Successful(string url)
    {
        var source = new UrlSource();
        dynamic result = source.Parse(url);
        Assert.That(result, Is.AssignableTo<IDictionary<string, object>>());
        Assert.That(result, Does.ContainKey("Hostname"));
        Assert.That(result["Hostname"], Is.EqualTo("127.0.0.1"));
    }

    [Test]
    [TestCase("https://localhost")]
    [TestCase("https://localhost/page.html")]
    [TestCase("https://localhost/path/page.html")]
    public void Parse_HostNameAsFriendlyName_Successful(string url)
    {
        var source = new UrlSource();
        dynamic result = source.Parse(url);
        Assert.That(result, Is.AssignableTo<IDictionary<string, object>>());
        Assert.That(result, Does.ContainKey("Hostname"));
        Assert.That(result["Hostname"], Is.EqualTo("localhost"));
    }

    [Test]
    [TestCase("https://localhost:8080")]
    public void Parse_Port_Successful(string url)
    {
        var source = new UrlSource();
        dynamic result = source.Parse(url);
        Assert.That(result, Is.AssignableTo<IDictionary<string, object>>());
        Assert.That(result, Does.ContainKey("Port"));
        Assert.That(result["Port"], Is.EqualTo(8080));
    }

    [Test]
    [TestCase("https://localhost")]
    public void Parse_PortMissing_Successful(string url)
    {
        var source = new UrlSource();
        dynamic result = source.Parse(url);
        Assert.That(result, Is.AssignableTo<IDictionary<string, object>>());
        Assert.That(result, Does.Not.ContainKey("Port"));
    }

    [Test]
    [TestCase("https://localhost", 0)]
    [TestCase("https://localhost/page.html", 1)]
    [TestCase("https://localhost/path/page.html", 2)]
    [TestCase("https://localhost/path/to/page.html", 3)]
    public void Parse_Paths_Successful(string url, int count)
    {
        var source = new UrlSource();
        dynamic result = source.Parse(url);
        Assert.That(result, Is.AssignableTo<IDictionary<string, object>>());
        Assert.That(result, Does.ContainKey("Paths"));
        Assert.That(result["Paths"], Is.AssignableTo<List<string>>());
        Assert.That(result["Paths"], Has.Count.EqualTo(count));
    }

    [Test]
    [TestCase("https://localhost/?q1=v1", 1)]
    [TestCase("https://localhost/?q1=v1&q2=v2", 2)]
    [TestCase("https://localhost/?q1=v1&q2=v2&q3=v3", 3)]
    public void Parse_QueryStrings_Successful(string url, int count)
    {
        var source = new UrlSource();
        dynamic result = source.Parse(url);
        Assert.That(result, Is.AssignableTo<IDictionary<string, object>>());
        Assert.That(result, Does.ContainKey("QueryStrings"));
        Assert.That(result["QueryStrings"], Is.AssignableTo<IDictionary<string, string>>());
        Assert.That(result["QueryStrings"], Has.Count.EqualTo(count));
        for (int i = 0; i < count; i++)
        {
            Assert.That(result["QueryStrings"], Does.ContainKey($"q{i + 1}"));
            Assert.That(result["QueryStrings"][$"q{i + 1}"], Is.EqualTo($"v{i + 1}"));
        }
    }

    [Test]
    [TestCase("https://localhost")]
    public void Parse_QueryStringsMissing_Successful(string url)
    {
        var source = new UrlSource();
        dynamic result = source.Parse(url);
        Assert.That(result, Is.AssignableTo<IDictionary<string, object>>());
        Assert.That(result, Does.Not.ContainKey("QueryStrings"));
    }

    [Test]
    [TestCase("https://localhost/pahe.html#section")]
    public void Parse_Fragment_Successful(string url)
    {
        var source = new UrlSource();
        dynamic result = source.Parse(url);
        Assert.That(result, Is.AssignableTo<IDictionary<string, object>>());
        Assert.That(result, Does.ContainKey("Fragment"));
        Assert.That(result["Fragment"], Is.EqualTo("#section"));
    }

    [Test]
    [TestCase("https://localhost")]
    public void Parse_FragmentMissing_Successful(string url)
    {
        var source = new UrlSource();
        dynamic result = source.Parse(url);
        Assert.That(result, Is.AssignableTo<IDictionary<string, object>>());
        Assert.That(result, Does.Not.ContainKey("Fragment"));
    }

    [Test]
    public void Parse_Stream_Successful()
    {
        var source = new UrlSource();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("https://www.domain.com"));
        dynamic result = source.Parse(stream);
        Assert.That(result, Is.AssignableTo<IDictionary<string, object>>());
        Assert.That(result, Does.ContainKey("Scheme"));
        Assert.That(result["Scheme"], Is.EqualTo("https"));
        Assert.That(result, Does.ContainKey("Hostname"));
        Assert.That(result["Hostname"], Is.EqualTo("www.domain.com"));
    }
}
