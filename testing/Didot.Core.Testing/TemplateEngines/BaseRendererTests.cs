using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.TemplateEngines;
using NUnit.Framework;

namespace Didot.Core.Testing.TemplateEngines;
public abstract class BaseRendererTests
{
    protected abstract ITemplateEngine GetEngine();

    [Test]
    public abstract void Render_SingleProperty_Successful();
    protected void Render_SingleProperty_Successful(string template)
    {
        var engine = GetEngine();
        var renderer = engine.Prepare(template);
        
        Assert.That(renderer.Render(new { model = new Dictionary<string, object> { { "Name", "World"} } })
            , Is.EqualTo("Hello World"));
        Assert.That(renderer.Render(new { model = new Dictionary<string, object> { { "Name", "Cédric" } } })
            , Is.EqualTo("Hello Cédric"));
    }
}
