namespace Didot.Core;

public interface IPipeline<TContext> where TContext : IPipelineContext
{
    void Execute(TContext context);
}
