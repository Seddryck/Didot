namespace Didot.Core;

public interface IPipelineStep<in TContext> where TContext : IPipelineContext
{
    void Execute(TContext context);
}
