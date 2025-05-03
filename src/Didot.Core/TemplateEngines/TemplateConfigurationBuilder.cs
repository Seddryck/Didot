using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core.TemplateEngines;
public class TemplateConfigurationBuilder
{
    private bool _htmlEncode = false;
    private bool _wrapAsModel = true;

    public TemplateConfigurationBuilder WithHtmlEncode()
    {
        _htmlEncode = true;
        return this;
    }

    public TemplateConfigurationBuilder WithoutHtmlEncode()
    {
        _htmlEncode = false;
        return this;
    }

    public TemplateConfigurationBuilder WithWrapAsModel()
    {
        _wrapAsModel = true;
        return this;
    }

    public TemplateConfigurationBuilder WithoutWrapAsModel()
    {
        _wrapAsModel = false;
        return this;
    }

    public TemplateConfiguration Build()
        => new (_htmlEncode, _wrapAsModel);
}
