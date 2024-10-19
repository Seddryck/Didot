using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.DataSourceEngines;
using NUnit.Framework;

namespace Didot.Core.Testing.DataSourceEngines;
public class FileBasedSourceEngineFactoryTests
{
    [Test]
    [TestCase(".json", typeof(JsonSource))]
    [TestCase(".yaml", typeof(YamlSource))]
    [TestCase(".yml", typeof(YamlSource))]
    [TestCase(".xml", typeof(XmlSource))]
    public void GetSourceParser_Extension_CorrectParser(string extension, Type expected)
    {
        var factory = new FileBasedSourceEngineFactory();
        var parser = factory.GetSourceParser(extension);
        Assert.That(parser, Is.TypeOf(expected));
    }

    [Test]
    [TestCase(".toml")]
    public void GetSourceParser_NotSupportedExtension_Exception(string extension)
    {
        var factory = new FileBasedSourceEngineFactory();
        Assert.Throws<NotSupportedException>(() => factory.GetSourceParser(extension));
    }
}
