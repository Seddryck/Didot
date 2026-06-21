namespace Didot.Core;

public interface IPipelineStep<TContext> where TContext : IPipelineContext
{
    void Execute(TContext context);
}
