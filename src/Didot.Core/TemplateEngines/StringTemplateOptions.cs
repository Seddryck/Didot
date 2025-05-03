using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core.TemplateEngines;
public record struct StringTemplateOptions(
    StringTemplateOptions.CharCouple Delimiters
) : ITemplateEngineOptions
{
    public record struct CharCouple(char Left, char Right) { }

    private static readonly StringTemplateOptions _default = new StringTemplateOptions(new CharCouple('<', '>'));
    public static readonly StringTemplateOptions Default = _default;
}
