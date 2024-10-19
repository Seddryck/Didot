using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core.DataSourceEngines;
public class FileBasedSourceEngineFactory
{
    public ISourceParser GetSourceParser(string extension)
        => extension.ToLowerInvariant() switch
        {
            ".json" => new JsonSource(),
            ".yaml" or ".yml" => new YamlSource(),
            _ => throw new NotSupportedException()
        };
}
