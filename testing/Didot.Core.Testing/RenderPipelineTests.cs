using Moq;
using NUnit.Framework;

namespace Didot.Core.Testing;

public class RenderPipelineTests
{
    [Test]
    public void Execute_StringTemplateAndStringInput_ParsesThenRenders()
    {
        var parser = new Mock<ISourceParser>();
        parser.Setup(p => p.Parse("Name: World")).Returns(new Dictionary<string, object> { ["Name"] = "World" });

        var engine = new Mock<ITemplateEngine>();
        engine.Setup(e => e.Render("Hello {{model.Name}}", It.IsAny<object>())).Returns("Hello World");

        var pipeline = new RenderPipeline();
        var context = new RenderPipelineContext
        {
            TemplateEngine = engine.Object,
            Template = "Hello {{model.Name}}",
            Inputs = new Dictionary<string, IModelInput>
            {
                [string.Empty] = new StringModelInput("Name: World", parser.Object)
            },
        };

        pipeline.Execute(context);

        Assert.That(context.Output, Is.EqualTo("Hello World"));
        parser.Verify(p => p.Parse("Name: World"), Times.Once);
        engine.Verify(e => e.Render("Hello {{model.Name}}", It.IsAny<object>()), Times.Once);
    }

    [Test]
    public void Execute_WithMultipleHooks_AppliesHooksInOrder()
    {
        var parser = new Mock<ISourceParser>();
        parser.Setup(p => p.Parse("{}")).Returns(new Dictionary<string, object> { ["value"] = 2 });

        var order = new List<string>();

        var engine = new Mock<ITemplateEngine>();
        engine
            .Setup(e => e.Render(It.IsAny<string>(), It.IsAny<object>()))
            .Callback<string, object>((_, wrappedModel) =>
            {
                var model = (IDictionary<string, object>)wrappedModel.GetType().GetProperty("model")!.GetValue(wrappedModel)!;
                Assert.That(model["value"], Is.EqualTo(7));
            })
            .Returns("ok");

        var context = new RenderPipelineContext
        {
            TemplateEngine = engine.Object,
            Template = "ignored",
            Inputs = new Dictionary<string, IModelInput>
            {
                [string.Empty] = new StringModelInput("{}", parser.Object)
            },
            Hooks =
            [
                new DelegateHook(model =>
                {
                    order.Add("hook-1");
                    var dict = (IDictionary<string, object>)model;
                    dict["value"] = (int)dict["value"] * 3;
                    return dict;
                }),
                new DelegateHook(model =>
                {
                    order.Add("hook-2");
                    var dict = (IDictionary<string, object>)model;
                    dict["value"] = (int)dict["value"] + 1;
                    return dict;
                })
            ]
        };

        new RenderPipeline().Execute(context);

        Assert.That(context.Output, Is.EqualTo("ok"));
        Assert.That(order, Is.EqualTo(new[] { "hook-1", "hook-2" }));
    }

    [Test]
    public void Execute_WhenHookReturnsNull_Throws()
    {
        var parser = new Mock<ISourceParser>();
        parser.Setup(p => p.Parse("{}")).Returns(new Dictionary<string, object>());

        var engine = new Mock<ITemplateEngine>();
        var context = new RenderPipelineContext
        {
            TemplateEngine = engine.Object,
            Template = "ignored",
            Inputs = new Dictionary<string, IModelInput>
            {
                [string.Empty] = new StringModelInput("{}", parser.Object)
            },
            Hooks = [new DelegateHook(_ => null!)]
        };

        Assert.That(() => new RenderPipeline().Execute(context), Throws.TypeOf<InvalidOperationException>());
        engine.Verify(e => e.Render(It.IsAny<string>(), It.IsAny<object>()), Times.Never);
    }

    private sealed class DelegateHook(Func<object, object> apply) : IPipelineExtensionHook
    {
        private Func<object, object> ApplyDelegate { get; } = apply;

        public object Apply(object model)
            => ApplyDelegate(model);
    }
}
