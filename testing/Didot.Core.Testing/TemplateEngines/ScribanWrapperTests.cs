using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.TemplateEngines;
using NUnit.Framework;
using Scriban;
using Scriban.Runtime;

namespace Didot.Core.Testing.TemplateEngines;
public class ScribanWrapperTests : BaseTemplateWrapperTests
{
    protected override ITemplateEngine GetEngine()
        => new ScribanWrapper();
    protected override ITemplateEngine GetEngine(TemplateConfiguration config)
        => new ScribanWrapper(config);
    protected override ITemplateEngine GetEngine(ITemplateEngineOptions options)
        => throw new NotSupportedException();

    [Test]
    public override void Render_SingleProperty_Successful()
        => Render_SingleProperty_Successful("Hello {{model.Name}}");

    [Test]
    public override void Render_SinglePropertyWithOptions_Successful()
        => Assert.Ignore("Scriban wrapper does not support options");

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
        => Render_NestedProperties_Successful("Hello {{model.Name.First}} {{model.Name.Last}}. Your age is {{model.Age}} years old.");

    [Test]
    public override void Render_ArrayItems_Successful()
        => Render_ArrayItems_Successful("Hello {{model[0].Name}}. Your colleague is {{model[1].Age}} years old.");

    [Test]
    public override void Render_ArrayLoop_Successful()
        => Render_ArrayLoop_Successful("Hello {{ for item in model; item.Name; if !for.last; \", \"; end; end }}!");

    [Test]
    public override void Render_Stream_Successful()
        => Render_Stream_Successful("Hello {{model.Name}}");

    [Test]
    public override void Render_Dictionary_Successful()
        => Render_Dictionary_Successful("Greetings: {{ model.Lang | greetings }} {{ model.Name }}");

    [Test]
    public override void Render_Formatter_Successful()
        => Render_Formatter_Successful("Greetings: {{model.Name | upper}}");

    [Test]
    public override void Render_NamedTemplateFunction_Successful()
        => Render_NamedTemplate_Successful("Greetings: {{ Hello(model.Name.First, model.Name.Last) }}!",
            new KeyValuePair<string, string>("Hello", "{{ func Hello(firstName, lastName) -}}\r\n\tMr. {{ lastName }} {{ firstName -}}\r\n{{ end }}"));

    [Test]
    public override void Render_Partial_Successful()
        => Render_Partial_Successful("{{ include 'Hello' }}, {{ model.Name.First }} {{ model.Name.Last}}!",
            new KeyValuePair<string, string>("Hello", "Welcome"));

    [Test]
    public override void Render_NamedTemplateRename_Successful()
        => Assert.Ignore("Scriban wrapper does not support fully named templates");
}
