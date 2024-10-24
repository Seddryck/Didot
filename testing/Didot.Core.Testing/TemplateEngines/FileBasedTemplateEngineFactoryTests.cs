using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.TemplateEngines;
using NUnit.Framework;

namespace Didot.Core.Testing.TemplateEngines;
public class FileBasedTemplateEngineFactoryTests
{
    [Test]
    [TestCase(".scriban", typeof(ScribanWrapper))]
    [TestCase(".liquid", typeof(DotLiquidWrapper))]
    [TestCase(".hbs", typeof(HandlebarsWrapper))]
    [TestCase(".smart", typeof(SmartFormatWrapper))]
    [TestCase(".st", typeof(StringTemplateWrapper))]
    [TestCase(".stg", typeof(StringTemplateWrapper))]
    public void GetSourceParser_Extension_CorrectParser(string extension, Type expected)
    {
        var factory = new FileBasedTemplateEngineFactory();
        var parser = factory.GetTemplateEngineByExtension(extension);
        Assert.That(parser, Is.TypeOf(expected));
    }

    [Test]
    [TestCase(".template")]
    public void GetSourceParser_NotSupportedExtension_Exception(string extension)
    {
        var factory = new FileBasedTemplateEngineFactory();
        Assert.Throws<NotSupportedException>(() => factory.GetTemplateEngineByExtension(extension));
    }

    [Test]
    public void GetSourceParser_AddedSourceParser_Exception()
    {
        var factory = new FileBasedTemplateEngineFactory();
        factory.AddOrReplaceEngine(".txt", new HandlebarsWrapper());
        var parser = factory.GetTemplateEngineByExtension(".txt");
        Assert.That(parser, Is.TypeOf<HandlebarsWrapper>());
    }

    [Test]
    [TestCase("scriban", typeof(ScribanWrapper))]
    [TestCase("dotliquid", typeof(DotLiquidWrapper))]
    [TestCase("handlebars", typeof(HandlebarsWrapper))]
    [TestCase("smartformat", typeof(SmartFormatWrapper))]
    [TestCase("stringtemplate", typeof(StringTemplateWrapper))]
    [TestCase("fluid", typeof(FluidWrapper))]
    public void GetSourceParser_ByTag_Successful(string tag, Type expected)
    {
        var factory = new FileBasedTemplateEngineFactory();
        var parser = factory.GetTemplateEngineByTag(tag);
        Assert.That(parser, Is.TypeOf(expected));
    }
}
