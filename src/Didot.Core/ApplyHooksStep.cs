namespace Didot.Core;

public class ApplyHooksStep : IPipelineStep<RenderPipelineContext>
{
    public void Execute(RenderPipelineContext context)
    {
        var model = context.Model;
        foreach (var hook in context.Hooks)
            model = hook.Apply(model) ?? throw new InvalidOperationException($"Hook '{hook.GetType().Name}' returned a null model.");

        context.Model = model;
    }
}
