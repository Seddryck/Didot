using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core;
public interface ISource
{
    ISourceParser Parser { get; }
    Stream Content { get; }
}

public class Source : ISource
{
    public ISourceParser Parser { get; }
    public Stream Content { get; }

    public Source(Stream content, ISourceParser parser)
        => (Content, Parser) = (content, parser);
}
