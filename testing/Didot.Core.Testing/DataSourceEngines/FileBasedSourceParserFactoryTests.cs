using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.SourceParsers;
using NUnit.Framework;

namespace Didot.Core.Testing.DataSourceEngines;
public class FileBasedSourceParserFactoryTests
{
    [Test]
    [TestCase(".json", typeof(JsonSource))]
    [TestCase(".yaml", typeof(YamlSource))]
    [TestCase(".yml", typeof(YamlSource))]
    [TestCase(".xml", typeof(XmlSource))]
    public void GetSourceParser_Extension_CorrectParser(string extension, Type expected)
    {
        var factory = new FileBasedSourceParserFactory();
        var parser = factory.GetSourceParser(extension);
        Assert.That(parser, Is.TypeOf(expected));
    }

    [Test]
    [TestCase(".toml")]
    public void GetSourceParser_NotSupportedExtension_Exception(string extension)
    {
        var factory = new FileBasedSourceParserFactory();
        Assert.Throws<NotSupportedException>(() => factory.GetSourceParser(extension));
    }
}
