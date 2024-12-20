using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core.SourceParsers;
public abstract class BaseSourceBuilder<T> : ISourceBuilder<T> where T : ISourceParser
{
    public bool CanHandle(Type type)
        => typeof(T) == type;

    public abstract ISourceParser Build(IDictionary<string, string> parameters, string extension);
}
