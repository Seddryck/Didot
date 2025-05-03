using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Didot.Core.TemplateEngines;
using NUnit.Framework;

namespace Didot.Core.Testing.TemplateEngines;
public class HandlebarsWrapperTests : BaseTemplateWrapperTests
{
    protected override ITemplateEngine GetEngine()
        => new HandlebarsWrapper();
    protected override ITemplateEngine GetEngine(TemplateConfiguration config)
        => new HandlebarsWrapper(config);
    protected override ITemplateEngine GetEngine(ITemplateEngineOptions options)
        => throw new NotSupportedException();

    [Test]
    public override void Render_SingleProperty_Successful()
        => Render_SingleProperty_Successful("Hello {{model.Name}}");

    [Test]
    public override void Render_SinglePropertyWithOptions_Successful()
        => Assert.Ignore("Handlebars wrapper does not support options");

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
    public override void Render_MultiPropertyWrapAsModel_Successful()
        => Render_MultiPropertyWrapAsModel_Successful("Hello {{model.Name}}. You're {{model.Age}} years old.");

    [Test]
    public override void Render_MultiPropertyWithoutWrapAsModel_Successful()
        => Render_MultiPropertyWithoutWrapAsModel_Successful("Hello {{Name}}. You're {{Age}} years old.");

    [Test]
    public override void Render_NestedProperties_Successful()
        => Render_NestedProperties_Successful("{{#with model}}Hello {{#with Name}}{{First}} {{Last}}{{/with}}. Your age is {{Age}} years old.{{/with}}");

    [Test]
    public override void Render_ArrayItems_Successful()
        => Render_ArrayItems_Successful("Hello {{model.0.Name}}. Your colleague is {{model.1.Age}} years old.");

    [Test]
    public override void Render_ArrayLoop_Successful()
        => Render_ArrayLoop_Successful("Hello {{#each model}}{{Name}}{{#unless @last}}, {{/unless}}{{/each}}!");

    [Test]
    public override void Render_Stream_Successful()
        => Render_Stream_Successful("Hello {{model.Name}}");

    [Test]
    public override void Render_Dictionary_Successful()
        => Render_Dictionary_Successful("Greetings: {{greetings model.Lang}} {{model.Name}}");

    [Test]
    public override void Render_Formatter_Successful()
        => Render_Formatter_Successful("Greetings: {{upper model.Name}}");

    [Test]
    public override void Render_NamedTemplateFunction_Successful()
        => Assert.Ignore("Handlebars wrapper does not support named templates");

    [Test]
    public override void Render_Partial_Successful()
        => Render_Partial_Successful("{{> Greetings }}, {{model.Name.First}} {{model.Name.Last}}!",
            new KeyValuePair<string, string>("Greetings", "Welcome"));

    [Test]
    public override void Render_NamedTemplateRename_Successful()
        => Assert.Ignore("Handlebars wrapper does not support named templates");
}
