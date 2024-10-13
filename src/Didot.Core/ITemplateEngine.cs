using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core;
public interface ITemplateEngine
{
    string Render(string template, object model);
    string Render(StreamReader template, object model);
}
