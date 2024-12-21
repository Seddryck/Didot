using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core.SourceParsers;

[Extension(".json")]
internal class JsonSourceBuilder : BaseSourceBuilder<JsonSource>
{
    public override ISourceParser Build(IDictionary<string, string> parameters, string extension)
        => new JsonSource();
}
