using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core.SourceParsers;
public class FileBasedSourceParserFactory
{
    public ISourceParser GetSourceParser(string extension)
        => extension.ToLowerInvariant() switch
        {
            ".json" => new JsonSource(),
            ".yaml" or ".yml" => new YamlSource(),
            ".xml" => new XmlSource(),
            _ => throw new NotSupportedException()
        };
}
