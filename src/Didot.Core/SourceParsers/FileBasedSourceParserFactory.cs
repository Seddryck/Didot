using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core.SourceParsers;
public class FileBasedSourceParserFactory : BaseFactory<ISourceParser>
{
    protected override string ClassToken => "Source";

    protected override void Initialize()
    {
        items.Clear();
        items.Add(".json", new JsonSource());
        items.Add(".yaml", new YamlSource());
        items.Add(".yml", new YamlSource());
        items.Add(".csv", new CsvSource());
        items.Add(".xml", new XmlSource());
        items.Add(".url", new UrlSource());
        items.Add(".md", new FrontmatterMarkdownSource());
    }
}
