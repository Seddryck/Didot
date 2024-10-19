using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.TemplateEngines;
using NUnit.Framework;

namespace Didot.Core.Testing.TemplateEngines;
public class DotLiquidWrapperTests
{
    [Test]
    public void Render_SingleProperty_Successful()
    {
        var engine = new DotLiquidWrapper();
        var model = new Dictionary<string, object>()
            { { "Name", "World"} };
        var result = engine.Render("Hello {{model.Name}}", new { model });
        Assert.That(result, Is.EqualTo("Hello World"));
    }

    [Test]
    public void Render_MultiProperty_Successful()
    {
        var engine = new DotLiquidWrapper();
        var model = new Dictionary<string, object>()
            { { "Name", "Albert"}, {"Age", 30 } };
        var result = engine.Render("Hello {{model.Name}}. You're {{model.Age}} years old.", new { model });
        Assert.That(result, Is.EqualTo("Hello Albert. You're 30 years old."));
    }

    [Test]
    public void Render_NestedProperties_Successful()
    {
        var engine = new DotLiquidWrapper();
        var name = new Dictionary<string, object>()
            { { "First", "Albert"}, {"Last", "Einstein" } };
        var model = new Dictionary<string, object>()
            { { "Name", name}, {"Age", 30 } };
        var result = engine.Render("Hello {{model.Name.First}} {{model.Name.Last}}. Your age is {{model.Age}} years old.", new { model });
        Assert.That(result, Is.EqualTo("Hello Albert Einstein. Your age is 30 years old."));
    }

    [Test]
    public void Render_ArrayItems_Successful()
    {
        var engine = new DotLiquidWrapper();
        var albert = new Dictionary<string, object>()
            { { "Name", "Albert"}, {"Age", 30 } };
        var nikola = new Dictionary<string, object>()
            { { "Name", "Nikola"}, {"Age", 50 } };
        var model = new[] { albert, nikola };
        var result = engine.Render("Hello {{model[0].Name}}. Your colleague is {{model[1].Age}} years old.", new { model });
        Assert.That(result, Is.EqualTo("Hello Albert. Your colleague is 50 years old."));
    }

    [Test]
    public void Render_ArrayLoop_Successful()
    {
        var engine = new DotLiquidWrapper();
        var albert = new Dictionary<string, object>()
            { { "Name", "Albert"}, {"Age", 30 } };
        var nikola = new Dictionary<string, object>()
            { { "Name", "Nikola"}, {"Age", 50 } };
        var model = new[] { albert, nikola };
        var result = engine.Render("Hello {% for item in model %}{{ item.Name }}{% if forloop.last == false %}, {% endif %}{% endfor %}!", new { model });
        Assert.That(result, Is.EqualTo("Hello Albert, Nikola!"));
    }

    [Test]
    public void Render_Stream_Successful()
    {
        var engine = new DotLiquidWrapper();
        var model = new Dictionary<string, object>()
            { { "Name", "World"} };
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello {{model.Name}}"));
        var result = engine.Render(stream, new { model });
        Assert.That(result, Is.EqualTo("Hello World"));
    }
}
