using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core.SourceParsers;
internal interface ISourceBuilder<T> : ISourceBuilder where T : ISourceParser
{
    bool CanHandle(Type type);
}

internal interface ISourceBuilder
{
    ISourceParser Build(IDictionary<string, string> parameters, string extension);
}
