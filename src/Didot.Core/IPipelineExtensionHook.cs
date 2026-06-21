namespace Didot.Core;

public interface IPipelineExtensionHook
{
    object Apply(object model);
}
