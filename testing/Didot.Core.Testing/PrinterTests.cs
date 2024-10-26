using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace Didot.Core.Testing;
public class PrinterTests
{
    [Test]
    public void Render_StringString_Successful()
    {
        var parser = new Mock<ISourceParser>();
        parser.Setup(p => p.Parse(It.IsAny<string>())).Returns(new Dictionary<string, object>() { { "Name", "World" } });
        var engine = new Mock<ITemplateEngine>();
        engine.Setup(e => e.Render(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>())).Returns("Hello World");
        var printer = new Printer(engine.Object);
        printer.Render("Hello {{model.Name}}", "Name: World", parser.Object);
        parser.Verify(p => p.Parse("Name: World"), Times.Once);
        engine.Verify(e => e.Render("Hello {{model.Name}}", It.IsAny<object>()), Times.Once);
    }

    [Test]
    public void Render_StringStream_Successful()
    {
        var parser = new Mock<ISourceParser>();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("Name: World"));
        parser.Setup(p => p.Parse(It.IsAny<Stream>())).Returns(new Dictionary<string, object>() { { "Name", "World" } });
        var engine = new Mock<ITemplateEngine>();
        engine.Setup(e => e.Render(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>())).Returns("Hello World");
        var printer = new Printer(engine.Object);
        printer.Render("Hello {{model.Name}}", stream, parser.Object);
        parser.Verify(p => p.Parse(stream), Times.Once);
        engine.Verify(e => e.Render("Hello {{model.Name}}", It.IsAny<object>()), Times.Once);
    }

    [Test]
    public void Render_StreamStream_Successful()
    {
        var parser = new Mock<ISourceParser>();
        using var streamData = new MemoryStream(Encoding.UTF8.GetBytes("Name: World"));
        using var streamTemplate = new MemoryStream(Encoding.UTF8.GetBytes("Hello {{model.Name}}"));
        parser.Setup(p => p.Parse(It.IsAny<Stream>())).Returns(new Dictionary<string, object>() { { "Name", "World" } });
        var engine = new Mock<ITemplateEngine>();
        engine.Setup(e => e.Render(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>())).Returns("Hello World");
        var printer = new Printer(engine.Object);
        printer.Render(streamTemplate, streamData, parser.Object);
        parser.Verify(p => p.Parse(streamData), Times.Once);
        engine.Verify(e => e.Render(streamTemplate, It.IsAny<object>()), Times.Once);
    }

    [Test]
    public void Render_StreamString_Successful()
    {
        var parser = new Mock<ISourceParser>();
        using var streamTemplate = new MemoryStream(Encoding.UTF8.GetBytes("Hello {{model.Name}}"));
        parser.Setup(p => p.Parse(It.IsAny<Stream>())).Returns(new Dictionary<string, object>() { { "Name", "World" } });
        var engine = new Mock<ITemplateEngine>();
        engine.Setup(e => e.Render(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>())).Returns("Hello World");
        var printer = new Printer(engine.Object);
        printer.Render(streamTemplate, "Name: World", parser.Object);
        parser.Verify(p => p.Parse("Name: World"), Times.Once);
        engine.Verify(e => e.Render(streamTemplate, It.IsAny<object>()), Times.Once);
    }
}
