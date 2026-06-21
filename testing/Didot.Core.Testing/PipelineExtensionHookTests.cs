using Moq;
using NUnit.Framework;

namespace Didot.Core.Testing;

public class PipelineExtensionHookTests
{
    [Test]
    public void Render_StringString_WithHook_EnrichesModel()
    {
        var parser = new Mock<ISourceParser>();
        parser.Setup(p => p.Parse(It.IsAny<string>())).Returns(new Dictionary<string, object>
        {
            ["firstName"] = "Albert",
            ["lastName"] = "Einstein",
        });

        var engine = new Mock<ITemplateEngine>();
        engine
            .Setup(e => e.Render(It.IsAny<string>(), It.IsAny<object>()))
            .Callback<string, object>((_, wrappedModel) =>
            {
                var model = (IDictionary<string, object>)wrappedModel.GetType().GetProperty("model")!.GetValue(wrappedModel)!;
                Assert.That(model["fullName"], Is.EqualTo("Albert Einstein"));
            })
            .Returns("ok");

        var printer = new Printer(engine.Object)
            .AddHook(new DelegateHook(model =>
            {
                var dict = (IDictionary<string, object>)model;
                dict["fullName"] = $"{dict["firstName"]} {dict["lastName"]}";
                return dict;
            }));

        var result = printer.Render("Hello {{model.fullName}}", "{}", parser.Object);

        Assert.That(result, Is.EqualTo("ok"));
    }

    [Test]
    public void Render_StringString_WithMultipleHooks_AppliesInOrder()
    {
        var parser = new Mock<ISourceParser>();
        parser.Setup(p => p.Parse(It.IsAny<string>())).Returns(new Dictionary<string, object>
        {
            ["value"] = 1,
        });

        var executionOrder = new List<string>();

        var engine = new Mock<ITemplateEngine>();
        engine
            .Setup(e => e.Render(It.IsAny<string>(), It.IsAny<object>()))
            .Callback<string, object>((_, wrappedModel) =>
            {
                var model = (IDictionary<string, object>)wrappedModel.GetType().GetProperty("model")!.GetValue(wrappedModel)!;
                Assert.That(model["value"], Is.EqualTo(5));
            })
            .Returns("ok");

        var firstHook = new DelegateHook(model =>
        {
            executionOrder.Add("hook-1");
            var dict = (IDictionary<string, object>)model;
            dict["value"] = (int)dict["value"] * 2;
            return dict;
        });

        var secondHook = new DelegateHook(model =>
        {
            executionOrder.Add("hook-2");
            var dict = (IDictionary<string, object>)model;
            dict["value"] = (int)dict["value"] + 3;
            return dict;
        });

        var printer = new Printer(engine.Object)
            .AddHook(firstHook)
            .AddHook(secondHook);

        var result = printer.Render("x", "{}", parser.Object);

        Assert.That(result, Is.EqualTo("ok"));
        Assert.That(executionOrder, Is.EqualTo(new[] { "hook-1", "hook-2" }));
    }

    [Test]
    public void Render_StringString_WhenHookReturnsNull_Throws()
    {
        var parser = new Mock<ISourceParser>();
        parser.Setup(p => p.Parse(It.IsAny<string>())).Returns(new Dictionary<string, object>());

        var engine = new Mock<ITemplateEngine>();
        var printer = new Printer(engine.Object)
            .AddHook(new DelegateHook(_ => null!));

        Assert.That(
            () => printer.Render("x", "{}", parser.Object),
            Throws.TypeOf<InvalidOperationException>());

        engine.Verify(e => e.Render(It.IsAny<string>(), It.IsAny<object>()), Times.Never);
    }

    private sealed class DelegateHook(Func<object, object> apply) : IPipelineExtensionHook
    {
        private Func<object, object> ApplyDelegate { get; } = apply;

        public object Apply(object model)
            => ApplyDelegate(model);
    }
}
