using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Didot.Core.SourceParsers;
internal class UrlSource : ISourceParser
{
    public object Parse(Stream content)
    {
        using var reader = new StreamReader(content);
        var url = reader.ReadToEnd();
        return Parse(url);
    }

    public object Parse(string url)
    {
        var result = new Dictionary<string, object>();
        var uri = new Uri(url);

        result["Scheme"] = uri.Scheme;
        result["Hostname"] = uri.Host;
        if (!uri.IsDefaultPort)
            result["Port"] = uri.Port;
        var paths = uri.AbsolutePath.Split("/", StringSplitOptions.RemoveEmptyEntries);
        result["Paths"] = new List<string>(paths);
        if (!string.IsNullOrEmpty(uri.Query))
        {
            var queryStrings = HttpUtility.ParseQueryString(uri.Query);
            var queryDictionary = queryStrings.AllKeys
                .ToDictionary(key => key ?? string.Empty, key => queryStrings[key]);
            result["QueryStrings"] = queryDictionary;
        }
        if (!string.IsNullOrEmpty(uri.Fragment))
            result["Fragment"] = uri.Fragment;
        return result;
    }
}
