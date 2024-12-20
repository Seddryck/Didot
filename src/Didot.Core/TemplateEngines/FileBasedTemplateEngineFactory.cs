using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.SourceParsers;
using Didot.Core.TemplateEngines;

namespace Didot.Core.TemplateEngines;
public class FileBasedTemplateEngineFactory : BaseFactory<ITemplateEngine>
{
    protected override string ClassToken => "Wrapper";

    protected override void Initialize(IDictionary<string, string> parameters)
    {
        items.Clear();
        items.Add(".scriban", new ScribanWrapper());
        items.Add(".liquid", new DotLiquidWrapper());
        items.Add(".hbs", new HandlebarsWrapper());
        items.Add(".smart", new SmartFormatWrapper());
        items.Add(".st", new StringTemplateWrapper());
        items.Add(".stg", new StringTemplateWrapper());
    }
}
