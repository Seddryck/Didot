using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core.SourceParsers;

[Extension(".url")]
internal class UrlSourceBuilder : BaseSourceBuilder<UrlSource>
{
    public override ISourceParser Build(IDictionary<string, string> parameters, string extension)
        => new UrlSource();
}
