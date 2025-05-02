using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Didot.Core.Testing.TemplateEngines;
public abstract class BaseTemplateWrapperTests
{
    protected abstract ITemplateEngine GetEngine();
    protected abstract ITemplateEngine GetEngine(TemplateConfiguration config);

    [Test]
    public abstract void Render_SingleProperty_Successful();
    protected void Render_SingleProperty_Successful(string template)
    {
        var engine = GetEngine();
        var model = new Dictionary<string, object>()
            { { "Name", "World"} };
        var result = engine.Render(template, new { model });
        Assert.That(result, Is.EqualTo("Hello World"));
    }

    [Test]
    public abstract void RenderWithoutEncode_QuotedProperty_Successful();
    protected void RenderWithoutEncode_QuotedProperty_Successful(string template)
    {
        var config = new TemplateConfiguration() { HtmlEncode = false };
        var engine = GetEngine(config);
        var model = new Dictionary<string, object>()
            { { "Name", "\"World\""} };
        var result = engine.Render(template, new { model });
        Assert.That(result, Is.EqualTo("Hello \"World\""));
    }

    [Test]
    public abstract void RenderWithEncode_QuotedProperty_Successful();
    protected void RenderWithEncode_QuotedProperty_Successful(string template)
    {
        var config = new TemplateConfiguration(HtmlEncode: true);
        var engine = GetEngine(config);
        var model = new Dictionary<string, object>()
            { { "Name", "\"World\""} };
        var result = engine.Render(template, new { model });
        Assert.That(result, Is.EqualTo("Hello &quot;World&quot;"));
    }

    [Test]
    public abstract void Render_MultiProperty_Successful();
    protected void Render_MultiProperty_Successful(string template)
    {
        var engine = GetEngine();
        var model = new Dictionary<string, object>()
            { { "Name", "Albert"}, {"Age", 30 } };
        var result = engine.Render(template, new { model });
        Assert.That(result, Is.EqualTo("Hello Albert. You're 30 years old."));
    }

    [Test]
    public abstract void Render_NestedProperties_Successful();
    protected void Render_NestedProperties_Successful(string template)
    {
        var engine = GetEngine();
        var name = new Dictionary<string, object>()
            { { "First", "Albert"}, {"Last", "Einstein" } };
        var model = new Dictionary<string, object>()
            { { "Name", name}, {"Age", 30 } };
        var result = engine.Render(template, new { model });
        Assert.That(result, Is.EqualTo("Hello Albert Einstein. Your age is 30 years old."));
    }

    [Test]
    public abstract void Render_ArrayItems_Successful();
    protected void Render_ArrayItems_Successful(string template)
    {
        var engine = GetEngine();
        var albert = new Dictionary<string, object>()
            { { "Name", "Albert"}, {"Age", 30 } };
        var nikola = new Dictionary<string, object>()
            { { "Name", "Nikola"}, {"Age", 50 } };
        var model = new[] { albert, nikola };
        var result = engine.Render(template, new { model });
        Assert.That(result, Is.EqualTo("Hello Albert. Your colleague is 50 years old."));
    }

    [Test]
    public abstract void Render_ArrayLoop_Successful();
    protected void Render_ArrayLoop_Successful(string template)
    {
        var engine = GetEngine();
        var albert = new Dictionary<string, object>()
            { { "Name", "Albert"}, {"Age", 30 } };
        var nikola = new Dictionary<string, object>()
            { { "Name", "Nikola"}, {"Nikola", 50 } };
        var model = new[] { albert, nikola };
        var result = engine.Render(template, new { model });
        Assert.That(result, Is.EqualTo("Hello Albert, Nikola!"));
    }

    [Test]
    public abstract void Render_Stream_Successful();
    protected void Render_Stream_Successful(string template)
    {
        var engine = GetEngine();
        var model = new Dictionary<string, object>()
            { { "Name", "World"} };
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(template));
        var result = engine.Render(stream, new { model });
        Assert.That(result, Is.EqualTo("Hello World"));
    }

    [Test]
    public abstract void Render_Dictionary_Successful();
    protected void Render_Dictionary_Successful(string template)
    {
        var engine = GetEngine();
        var model = new Dictionary<string, object>()
            { { "Name", "Alice"}, {"Lang", "fr" } };
        var dict = new Dictionary<string, object>()
            { { "fr", "Bonjour"}, {"en", "Hello" }, {"es", "Hola"} };
        engine.AddMappings("greetings", dict);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(template));
        var result = engine.Render(stream, new { model });
        Assert.That(result, Is.EqualTo("Greetings: Bonjour Alice"));
    }

    [Test]
    public abstract void Render_Formatter_Successful();
    protected void Render_Formatter_Successful(string template)
    {
        var engine = GetEngine();
        var model = new Dictionary<string, object>()
            { { "Name", "Alice"} };
        engine.AddFormatter("upper", (x) => (((string?)x)?.ToUpper() ?? string.Empty) + "!");
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(template));
        var result = engine.Render(stream, new { model });
        Assert.That(result, Is.EqualTo("Greetings: ALICE!"));
    }

    [Test]
    public abstract void Render_NamedTemplateFunction_Successful();
    public abstract void Render_NamedTemplateRename_Successful();
    
    protected void Render_NamedTemplate_Successful(string template, KeyValuePair<string, string> namedTemplate)
    {
        var engine = GetEngine();
        var name = new Dictionary<string, object>()
            { { "First", "Albert"}, {"Last", "Einstein" } };
        var model = new Dictionary<string, object>()
            { { "Name", name } };
        engine.AddFunction(namedTemplate.Key, () => namedTemplate.Value);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(template));
        var result = engine.Render(stream, new { model });
        Assert.That(result, Is.EqualTo("Greetings: Mr. Einstein Albert!"));
    }

    public abstract void Render_Partial_Successful();
    protected void Render_Partial_Successful(string template, KeyValuePair<string, string> includedTemplate)
    {
        var engine = GetEngine();
        var name = new Dictionary<string, object>()
            { { "First", "Albert"}, {"Last", "Einstein" } };
        var model = new Dictionary<string, object>()
            { { "Name", name } };
        engine.AddPartial(includedTemplate.Key, () => includedTemplate.Value);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(template));
        var result = engine.Render(stream, new { model });
        Assert.That(result, Is.EqualTo("Welcome, Albert Einstein!"));
    }
}
