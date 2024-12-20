using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core.SourceParsers;

[Extension(".md")]
internal class FrontmatterMarkdownSourceBuilder : BaseSourceBuilder<FrontmatterMarkdownSource>
{
    public override ISourceParser Build(IDictionary<string, string> parameters, string extension)
        => new FrontmatterMarkdownSource();
}
