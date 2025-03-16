using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.TemplateEngines;
using NUnit.Framework;

namespace Didot.Core.Testing.TemplateEngines;
public class FluidWrapperTests : DotLiquidWrapperTests
{
    protected override ITemplateEngine GetEngine()
        => new FluidWrapper();

    [Test]
    public override void Render_Dictionary_Successful()
    {
        var engine = GetEngine();
        var model = new Dictionary<string, object>()
            { { "Name", "Alice"}, {"Lang", "fr" } };
        var dict = new Dictionary<string, object>()
            { { "fr", "Bonjour"}, {"en", "Hello" }, {"es", "Hola"} };
        engine.AddMappings("greetings", dict);
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("Greetings: {{ model.Lang | greetings }} {{ model.Name }}"));
        var result = engine.Render(stream, new { model });
        Assert.That(result, Is.EqualTo("Greetings: Bonjour Alice"));
    }
}
