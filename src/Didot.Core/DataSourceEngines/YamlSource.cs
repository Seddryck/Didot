using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace Didot.Core.DataSourceEngines;
public class YamlSource : ISourceParser
{
    public object Parse(string content)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        dynamic yamlObject = deserializer.Deserialize<dynamic>(content);

        return yamlObject;
    }

    public object Parse(StreamReader reader)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        dynamic yamlObject = deserializer.Deserialize<dynamic>(reader);

        return yamlObject;
    }
}
