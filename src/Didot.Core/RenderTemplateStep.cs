namespace Didot.Core;

public class RenderTemplateStep : IPipelineStep<RenderPipelineContext>
{
    public void Execute(RenderPipelineContext context)
    {
        var wrappedModel = new { model = context.Model };

        if (context.Template is not null)
            context.Output = context.TemplateEngine.Render(context.Template, wrappedModel);
        else if (context.TemplateStream is not null)
            context.Output = context.TemplateEngine.Render(context.TemplateStream, wrappedModel);
        else
            throw new InvalidOperationException("No template content was provided to render.");
    }
}
