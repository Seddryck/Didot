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
    private TemplateConfiguration Configuration { get; }

    public FileBasedTemplateEngineFactory()
        : this(new TemplateConfiguration())
    { }

    public FileBasedTemplateEngineFactory(TemplateConfiguration config)
    {
        Configuration = config;
        Initialize(new Dictionary<string, string>());
    }

    protected override string ClassToken => "Wrapper";

    protected override void Initialize(IDictionary<string, string> parameters)
    {
        items.Clear();
        items.Add(".scriban", new ScribanWrapper(Configuration));
        items.Add(".liquid", new DotLiquidWrapper(Configuration));
        items.Add(".hbs", new HandlebarsWrapper(Configuration));
        items.Add(".smart", new SmartFormatWrapper(Configuration));
        items.Add(".st", new StringTemplateWrapper(Configuration));
        items.Add(".stg", items[".st"]);
        items.Add(".morestachio", new MorestachioWrapper(Configuration));
        items.Add(".mustache", items[".morestachio"]);
    }
}
