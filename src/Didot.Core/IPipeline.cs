namespace Didot.Core;

public interface IPipeline<in TContext> where TContext : IPipelineContext
{
    void Execute(TContext context);
}
