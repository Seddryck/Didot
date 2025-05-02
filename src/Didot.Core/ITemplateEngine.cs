using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core;
public interface ITemplateEngine
{
    void AddMappings(string mapKey, IDictionary<string, object> mappings);
    void AddFormatter(string name, Func<object?, string> function);
    void AddFunction(string name, Func<string> namedTemplate);
    void AddPartial(string name, Func<string> includedTemplate);
    string Render(string template, object model);
    string Render(Stream template, object model);
}
