using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartFormat;

namespace Didot.Core.TemplateEngines;
public class SmartFormatWrapper : ITemplateEngine
{
    public string Render(string template, object model)
        => Smart.Format(template, model);

    public string Render(Stream stream, object model)
    {
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        return Render(template, model);
    }
}
