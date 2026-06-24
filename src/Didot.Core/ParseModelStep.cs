namespace Didot.Core;

public class ParseModelStep : IPipelineStep<RenderPipelineContext>
{
    public void Execute(RenderPipelineContext context)
    {
        if (context.Model is not null)
            return;

        if (context.Inputs.Count == 0)
            throw new InvalidOperationException("No model inputs were provided to parse.");

        if (context.Inputs.Count == 1 && string.IsNullOrEmpty(context.Inputs.First().Key))
        {
            context.Model = context.Inputs.First().Value.Parse();
            return;
        }

        var model = new Dictionary<string, object>();
        foreach (var input in context.Inputs)
            model.Add(input.Key, input.Value.Parse());

        context.Model = model;
    }
}
