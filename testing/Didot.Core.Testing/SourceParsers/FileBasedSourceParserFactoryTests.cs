using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.SourceParsers;
using NUnit.Framework;

namespace Didot.Core.Testing.SourceParsers;
public class FileBasedSourceParserFactoryTests
{
    [Test]
    [TestCase(".json", typeof(JsonSource))]
    [TestCase(".yaml", typeof(YamlSource))]
    [TestCase(".yml", typeof(YamlSource))]
    [TestCase(".xml", typeof(XmlSource))]
    [TestCase(".url", typeof(UrlSource))]
    [TestCase(".md", typeof(FrontmatterMarkdownSource))]
    public void GetSourceParser_Extension_CorrectParser(string extension, Type expected)
    {
        var factory = new FileBasedSourceParserFactory();
        var parser = factory.GetByExtension(extension);
        Assert.That(parser, Is.TypeOf(expected));
    }

    [Test]
    [TestCase(".toml")]
    public void GetSourceParser_NotSupportedExtension_Exception(string extension)
    {
        var factory = new FileBasedSourceParserFactory();
        Assert.Throws<NotSupportedException>(() => factory.GetByExtension(extension));
    }

    [Test]
    public void GetSourceParser_AddedSourceParser_Exception()
    {
        var factory = new FileBasedSourceParserFactory();
        factory.AddOrReplace(".frontmatter", new FrontmatterSource());
        var parser = factory.GetByExtension(".frontmatter");
        Assert.That(parser, Is.TypeOf<FrontmatterSource>());
    }

    [Test]
    [TestCase("JSON", typeof(JsonSource))]
    [TestCase("YAML", typeof(YamlSource))]
    [TestCase("XML", typeof(XmlSource))]
    [TestCase("FrontmatterMarkdown", typeof(FrontmatterMarkdownSource))]
    [TestCase("Frontmatter", typeof(FrontmatterSource))]
    public void GetSourceParser_ByTag_Successful(string tag, Type expected)
    {
        var factory = new FileBasedSourceParserFactory();
        var parser = factory.GetByTag(tag);
        Assert.That(parser, Is.TypeOf(expected));
    }
}
