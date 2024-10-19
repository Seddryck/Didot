using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.Text.Json;
using YamlDotNet.Core.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Didot.Core.DataSourceEngines;
public class JsonSource : ISourceParser
{
    public object Parse(string content)
        => ParseToken(JToken.Parse(content))!;

    private static object? ParseToken(JToken token)
    {
        if (token is JArray jArray)
        {
            var list = new List<object?>();
            foreach (var item in jArray)
                list.Add(ParseToken(item));
            return list;
        }
        else if (token is JObject jObject)
        {
            var dict = new Dictionary<string, object?>();
            foreach (var prop in jObject.Properties())
                dict[prop.Name] = ParseToken(prop.Value);
            return dict;
        }
        else if (token is JValue jValue)
            return jValue.Value;

        throw new NotSupportedException();
    }

    public object Parse(Stream stream)
    {
        using (var streamReader = new StreamReader(stream))
        using (var jsonReader = new JsonTextReader(streamReader))
            return ParseToken(JToken.Load(jsonReader))!;
    }

    private static Dictionary<string, object> Parse(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            var dict = new Dictionary<string, object>();
            foreach (var property in element.EnumerateObject())
            {
                dict[property.Name] = Parse(property.Value);
            }
            return dict;
        }
        else
            throw new NotSupportedException();
    }
}
