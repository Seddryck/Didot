using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.TemplateEngines;
using NUnit.Framework;

namespace Didot.Core.Testing.TemplateEngines;
public class MorestachioWrapperTests : BaseTemplateWrapperTests
{
    protected override ITemplateEngine GetEngine()
        => new MorestachioWrapper();
    protected override ITemplateEngine GetEngine(TemplateConfiguration config)
        => new MorestachioWrapper(config);
    protected override ITemplateEngine GetEngine(ITemplateEngineOptions options)
        => throw new NotSupportedException();

    [Test]
    public override void Render_SingleProperty_Successful()
        => Render_SingleProperty_Successful("Hello {{model.Name}}");

    [Test]
    public override void Render_SinglePropertyWithOptions_Successful()
        => Assert.Ignore("Morestachio wrapper does not support options");

    [Test]
    public override void RenderWithoutEncode_QuotedProperty_Successful()
        => RenderWithoutEncode_QuotedProperty_Successful("Hello {{model.Name}}");

    [Test]
    public override void RenderWithEncode_QuotedProperty_Successful()
        => RenderWithEncode_QuotedProperty_Successful("Hello {{model.Name}}");

    [Test]
    public override void Render_MultiProperty_Successful()
        => Render_MultiProperty_Successful("Hello {{model.Name}}. You're {{model.Age}} years old.");

    [Test]
    public override void Render_NestedProperties_Successful()
        => Render_NestedProperties_Successful("Hello {{model.Name.First}} {{model.Name.Last}}. Your age is {{model.Age}} years old.");

    [Test]
    public override void Render_ArrayItems_Successful()
        => Render_ArrayItems_Successful("Hello {{model.ElementAt(0).Name}}. Your colleague is {{model.ElementAt(1).Age}} years old.");

    [Test]
    public override void Render_ArrayLoop_Successful()
        => Render_ArrayLoop_Successful("Hello {{#each model}}{{ Name }}{{#if !$last}}, {{/if}}{{/each}}!");

    [Test]
    public override void Render_Stream_Successful()
        => Render_Stream_Successful("Hello {{model.Name}}");

    [Test]
    public override void Render_Dictionary_Successful()
        => Render_Dictionary_Successful("Greetings: {{ model.Lang.greetings() }} {{ model.Name }}");

    [Test]
    public override void Render_Formatter_Successful()
        => Render_Formatter_Successful("Greetings: {{model.Name.upper()}}");

    [Test]
    public override void Render_NamedTemplateFunction_Successful()
        => Assert.Ignore("Morestachio wrapper does not support named templates");

    [Test]
    public override void Render_Partial_Successful()
        => Assert.Ignore("Morestachio wrapper does not support partials");

    [Test]
    public override void Render_NamedTemplateRename_Successful()
        => Assert.Ignore("Morestachio wrapper does not support named templates");
}
