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
    public void GetSourceParser_Extension_CorrectParser(string extension, Type expected)
    {
        var factory = new FileBasedTemplateEngineFactory();
        var parser = factory.GetTemplateEngine(extension);
        Assert.That(parser, Is.TypeOf(expected));
    }

    [Test]
    [TestCase(".template")]
    public void GetSourceParser_NotSupportedExtension_Exception(string extension)
    {
        var factory = new FileBasedTemplateEngineFactory();
        Assert.Throws<NotSupportedException>(() => factory.GetTemplateEngine(extension));
    }
}
