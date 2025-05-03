using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.TemplateEngines;
using HandlebarsDotNet;
using NUnit.Framework;

namespace Didot.Core.Testing;
public class TemplateEngineFactoryTests
{
    [Test]
    [TestCase(".liquid", typeof(DotLiquidWrapper))]
    [TestCase(".hbs", typeof(HandlebarsWrapper))]
    [TestCase(".stg", typeof(StringTemplateWrapper))]
    [TestCase(".st", typeof(StringTemplateWrapper))]
    [TestCase(".morestachio", typeof(MorestachioWrapper))]
    [TestCase(".mustache", typeof(MorestachioWrapper))]
    [TestCase(".smart", typeof(SmartFormatWrapper))]
    [TestCase(".scriban", typeof(ScribanWrapper))]
    public void Create_Default_Expected(string extension, Type expected)
    {
        var factory = TemplateEngineFactory.Default;
        var engine = factory.Create(extension);
        Assert.That(engine, Is.TypeOf(expected));
    }

    [Test]
    public void Create_Custom_Expected()
    {
        var factory = new TemplateEngineFactory();
        factory.AddOrReplace(".st", engine => engine.UseStringTemplate(opt => opt.WithDollarDelimitedExpressions()));
        var engine = factory.Create(".st");
        Assert.That(engine, Is.TypeOf<StringTemplateWrapper>());
        var stWrapper = (StringTemplateWrapper)engine;
        Assert.That(stWrapper.Options.Delimiters.Left, Is.EqualTo('$'));
        Assert.That(stWrapper.Options.Delimiters.Right, Is.EqualTo('$'));
    }

    [Test]
    public void ClearRemoveExists_Custom_Expected()
    {
        var factory = new TemplateEngineFactory();
        factory.AddOrReplace(".st", engine => engine.UseStringTemplate(opt => opt.WithDollarDelimitedExpressions()));
        factory.AddOrReplace(".scriban", engine => engine.UseScriban());
        Assert.That(factory.AllSupportedExtensions(), Has.Length.EqualTo(2));

        factory.Remove(".scriban");
        Assert.That(factory.AllSupportedExtensions(), Has.Length.EqualTo(1));
        Assert.That(factory.AllSupportedExtensions(), Has.Member(".st"));
        Assert.That(factory.Exists(".scriban"), Is.False);
        Assert.That(factory.Exists(".st"), Is.True);

        factory.Clear();
        Assert.That(factory.Count, Is.EqualTo(0));
        Assert.That(factory.Exists(".st"), Is.False);
    }
}
