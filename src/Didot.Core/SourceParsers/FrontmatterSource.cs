using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.IO;
using System.Reflection.Metadata.Ecma335;

namespace Didot.Core.SourceParsers;
public class FrontmatterSource : FrontmatterMarkdownSource
{
    public override object Parse(string content)
    {
        var lineDelimiter = GetLineDelimiter(content);
        var lines = content.Split(lineDelimiter);

        var metadataLines = new StringBuilder();
        Parse(lines, (string line) => metadataLines.AppendLine(line), null);

        return GetDeserializer()
                .Deserialize<dynamic>(
                    metadataLines.ToString()
                );
    }

    public override object Parse(Stream stream)
    {
        using (var reader = new StreamReader(stream))
            return Parse(reader.ReadToEnd());
    }
}
