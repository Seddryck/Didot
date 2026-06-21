namespace Didot.Core;

public class StringModelInput : IModelInput
{
    private string Content { get; }
    private ISourceParser Parser { get; }

    public StringModelInput(string content, ISourceParser parser)
        => (Content, Parser) = (content, parser);

    public object Parse()
        => Parser.Parse(Content);
}
