using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartFormat;

namespace Didot.Core.TemplateEngines;
public class SmartFormatWrapper : BaseTemplateEngine
{
    public SmartFormatWrapper()
        : base()
    { }

    public SmartFormatWrapper(TemplateConfiguration configuration)
        : base(configuration)
    { }

    public override void AddFormatter(string name, Func<object?, string> function)
        => throw new NotImplementedException();
    public override void AddMappings(string mapKey, IDictionary<string, object> mappings)
        => throw new NotImplementedException();
    public override void AddPartial(string name, Func<string> template)
        => throw new NotImplementedException();
    public override void AddFunction(string name, Func<string> template)
        => throw new NotImplementedException();

    public override string Render(string template, object model)
    {
        var context = CreateContext(model);
        return Smart.Format(template, context);
    }

    public override string Render(Stream stream, object model)
    {
        using var reader = new StreamReader(stream);
        var template = reader.ReadToEnd();
        return Render(template, model);
    }

    public override IRenderer Prepare(string template)
    {
        return new SmartFormatRenderer(template, CreateContext);
    }

    protected virtual object CreateContext(object model)
    {
        if (Configuration.WrapAsModel)
        {
            var isAlreadyWrapped = model.GetType().GetProperty("model") != null;
            if (!isAlreadyWrapped)
                model = new { model };
        }
        return model;
    }
}
