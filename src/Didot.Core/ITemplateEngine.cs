using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core;
public interface ITemplateEngine
{
    void AddMappings(string mapKey, IDictionary<string, object> mappings);
    string Render(string template, object model);
    string Render(Stream template, object model);
}
