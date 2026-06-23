namespace Didot.Core;

public class StreamModelInput : IModelInput
{
    private Stream Content { get; }
    private ISourceParser Parser { get; }

    public StreamModelInput(Stream content, ISourceParser parser)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(parser);
        (Content, Parser) = (content, parser);
    }

    public object Parse()
        => Parser.Parse(Content);
}
