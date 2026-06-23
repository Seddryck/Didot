namespace Didot.Core;

public class SourceModelInput : IModelInput
{
    private ISource Source { get; }

    public SourceModelInput(ISource source)
    {
        ArgumentNullException.ThrowIfNull(source);
        Source = source;
    }

    public object Parse()
        => Source.Parser.Parse(Source.Content);
}
