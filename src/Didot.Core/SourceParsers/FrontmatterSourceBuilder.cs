using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.IO;
using System.Reflection.Metadata.Ecma335;

namespace Didot.Core.SourceParsers;
internal class FrontmatterSourceBuilder : BaseSourceBuilder<FrontmatterSource>
{
    public override ISourceParser Build(IDictionary<string, string> parameters, string extension)
        => new FrontmatterSource();
}
