using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.IO;

namespace Didot.Core.SourceParsers;
public class YamlSource : ISourceParser
{
    protected static IDeserializer GetDeserializer()
        => new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

    public virtual object Parse(string content)
        => GetDeserializer().Deserialize<dynamic>(content);

    public virtual object Parse(Stream stream)
    {
        using (var reader = new StreamReader(stream))
        return GetDeserializer().Deserialize<dynamic>(reader);
    }
}
