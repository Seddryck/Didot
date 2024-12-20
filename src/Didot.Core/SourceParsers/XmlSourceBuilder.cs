using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core.SourceParsers;

[Extension(".xml")]
internal class XmlSourceBuilder : BaseSourceBuilder<XmlSource>
{
    public override ISourceParser Build(IDictionary<string, string> parameters, string extension)
        => new XmlSource();
}
