using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.TemplateEngines;

namespace Didot.Core.TemplateEngines;
public class FileBasedTemplateEngineFactory
{
    public ITemplateEngine GetTemplateEngine(string extension)
        => extension.ToLowerInvariant() switch
        {
            ".scriban" => new ScribanWrapper(),
            ".liquid" => new DotLiquidWrapper(),
            _ => throw new NotSupportedException()
        };
}
