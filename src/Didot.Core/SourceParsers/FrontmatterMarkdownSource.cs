using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.IO;

namespace Didot.Core.SourceParsers;
public class FrontmatterMarkdownSource : YamlSource
{
    protected string GetLineDelimiter(string content)
        => content.Contains("\r\n") ? "\r\n" : "\n";

    public override object Parse(string content)
    {
        var lineDelimiter = GetLineDelimiter(content);
        var lines = content.Split(lineDelimiter);

        var metadataLines = new StringBuilder();
        var contentLines = new StringBuilder();
        Parse(lines, (string line) => metadataLines.AppendLine(line), (string line) => contentLines.AppendLine(line));

        var result = GetDeserializer()
                                .Deserialize<dynamic>(
                                    metadataLines.ToString()
                                );
        result["Content"] = contentLines.Remove(contentLines.Length - 2, 2).ToString();
        return result;
    }

    protected void Parse(string[] lines, Action<string>? appendMetadata, Action<string>? appendContent)
    {
        bool isMetadata = false;
        bool isParsingMetadata = false;

        foreach (var line in lines)
        {
            if (line.Trim() == "---")
            {
                if (!isParsingMetadata)
                {
                    isParsingMetadata = true;
                    isMetadata = true;
                }
                else
                {
                    isParsingMetadata = false;
                    isMetadata = false;
                    continue; // Skip the closing '---' line
                }
            }

            if (isMetadata)
                appendMetadata?.Invoke(line);
            else
                appendContent?.Invoke(line);
        }
    }


    public override object Parse(Stream stream)
    {
        using (var reader = new StreamReader(stream))
            return Parse(reader.ReadToEnd());
    }
}
