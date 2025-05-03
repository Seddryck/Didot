using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotLiquid;

namespace Didot.Core.TemplateEngines;
public class DotLiquidWrapper : BaseTemplateEngine
{
    public DotLiquidWrapper()
        : base()
    { }

    public DotLiquidWrapper(TemplateConfiguration configuration)
        : base(configuration)
    { }

    public override void AddFormatter(string name, Func<object?, string> function)
        => throw new NotImplementedException();
    public override void AddFunction(string name, Func<string> template)
        => throw new NotImplementedException();
    public override void AddPartial(string name, Func<string> template)
        => throw new NotImplementedException();

    public override string Render(string source, object model)
    {
        var templateInstance = Template.Parse(source);
        var hash = Hash.FromAnonymousObject(model);

        foreach (var (dictName, dictValues) in Mappings)
            hash[dictName] = dictValues;

        return templateInstance.Render(hash);
    }

    public override string Render(Stream stream, object model)
    {
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        return Render(template, model);
    }
}
