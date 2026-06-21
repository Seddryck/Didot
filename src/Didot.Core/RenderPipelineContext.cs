namespace Didot.Core;

public class RenderPipelineContext : IPipelineContext
{
    public required ITemplateEngine TemplateEngine { get; init; }
    public string? Template { get; init; }
    public Stream? TemplateStream { get; init; }
    public IDictionary<string, IModelInput> Inputs { get; init; } = new Dictionary<string, IModelInput>();
    public IList<IPipelineExtensionHook> Hooks { get; init; } = [];
    public object? Model { get; set; }
    public string? Output { get; set; }
}
