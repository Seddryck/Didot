using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PocketCsvReader.Configuration;

namespace Didot.Core.TemplateEngines;
public class StringTemplateOptionsBuilder : ITemplateEngineOptionsBuilder
{
    private enum Delimiter
    {
        Dollar,
        AngleBracket
    }

    private Delimiter _delimiter = Delimiter.AngleBracket;

    public StringTemplateOptionsBuilder WithDollarDelimitedExpressions()
    {
        _delimiter = Delimiter.Dollar;
        return this;
    }

    public StringTemplateOptionsBuilder WithAngleBracketExpressions()
    {
        _delimiter = Delimiter.AngleBracket;
        return this;
    }

    public ITemplateEngineOptions Build()
        => _delimiter switch
        {
            Delimiter.Dollar => new StringTemplateOptions(new StringTemplateOptions.CharCouple('$', '$')),
            Delimiter.AngleBracket => new StringTemplateOptions(new StringTemplateOptions.CharCouple('<', '>')),
            _ => throw new InvalidOperationException("No delimiter style was specified.Call WithDollarDelimitedExpressions() or WithAngleBracketExpressions() before building.")
        };
}
