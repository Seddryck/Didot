using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.TemplateEngines;
using NUnit.Framework;

namespace Didot.Core.Testing;
public class TemplateEngineBuilderTests
{
    [Test]
    public void Build_UseStringTemplateWithoutOption_Expected()
    {
        var engine = new TemplateEngineBuilder()
            .UseStringTemplate()
            .Build();
        Assert.That(engine, Is.TypeOf<StringTemplateWrapper>());
        var stWrapper = (StringTemplateWrapper)engine;
        Assert.That(stWrapper.Options.Delimiters.Left, Is.EqualTo('<'));
        Assert.That(stWrapper.Options.Delimiters.Right, Is.EqualTo('>'));
    }

    [Test]
    public void Build_UseStringTemplateWithOptionDollar_Expected()
    {
        var engine = new TemplateEngineBuilder()
            .UseStringTemplate(options => options.WithDollarDelimitedExpressions())
            .Build();
        Assert.That(engine, Is.TypeOf<StringTemplateWrapper>());
        var stWrapper = (StringTemplateWrapper)engine;
        Assert.That(stWrapper.Options.Delimiters.Left, Is.EqualTo('$'));
        Assert.That(stWrapper.Options.Delimiters.Right, Is.EqualTo('$'));
    }

    [Test]
    public void Build_UseStringTemplateWithOptionAngleBracket_Expected()
    {
        var engine = new TemplateEngineBuilder()
            .UseStringTemplate(options => options.WithAngleBracketExpressions())
            .Build();
        Assert.That(engine, Is.TypeOf<StringTemplateWrapper>());
        var stWrapper = (StringTemplateWrapper)engine;
        Assert.That(stWrapper.Options.Delimiters.Left, Is.EqualTo('<'));
        Assert.That(stWrapper.Options.Delimiters.Right, Is.EqualTo('>'));
    }

    [Test]
    public void Build_UseHandlebarsWithoutOption_Expected()
    {
        var engine = new TemplateEngineBuilder()
            .UseHandlebars()
            .Build();
        Assert.That(engine, Is.TypeOf<HandlebarsWrapper>());
    }

    [Test]
    public void Build_UseDotLiquidWithoutOption_Expected()
    {
        var engine = new TemplateEngineBuilder()
            .UseDotLiquid()
            .Build();
        Assert.That(engine, Is.TypeOf<DotLiquidWrapper>());
    }

    [Test]
    public void Build_UseFluidWithoutOption_Expected()
    {
        var engine = new TemplateEngineBuilder()
            .UseFluid()
            .Build();
        Assert.That(engine, Is.TypeOf<FluidWrapper>());
    }

    [Test]
    public void Build_UseScribanWithoutOption_Expected()
    {
        var engine = new TemplateEngineBuilder()
            .UseScriban()
            .Build();
        Assert.That(engine, Is.TypeOf<ScribanWrapper>());
    }

    [Test]
    public void Build_UseMorestachioWithoutOption_Expected()
    {
        var engine = new TemplateEngineBuilder()
            .UseMorestachio()
            .Build();
        Assert.That(engine, Is.TypeOf<MorestachioWrapper>());
    }

    [Test]
    public void Build_UseSmartFormatWithoutOption_Expected()
    {
        var engine = new TemplateEngineBuilder()
            .UseSmartFormat()
            .Build();
        Assert.That(engine, Is.TypeOf<SmartFormatWrapper>());
    }
}
