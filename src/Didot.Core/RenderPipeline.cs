namespace Didot.Core;

public class RenderPipeline : IPipeline<RenderPipelineContext>
{
    private IList<IPipelineStep<RenderPipelineContext>> Steps { get; }

    public RenderPipeline()
        : this([
            new ParseModelStep(),
            new ApplyHooksStep(),
            new RenderTemplateStep(),
        ])
    { }

    public RenderPipeline(IEnumerable<IPipelineStep<RenderPipelineContext>> steps)
    {
        ArgumentNullException.ThrowIfNull(steps);
        Steps = [.. steps];
    }

    public void Execute(RenderPipelineContext context)
    {
        foreach (var step in Steps)
            step.Execute(context);
    }
}
