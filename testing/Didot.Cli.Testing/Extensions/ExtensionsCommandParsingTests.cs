using NUnit.Framework;

namespace Didot.Cli.Testing.Extensions;

public class ExtensionsCommandParsingTests
{
    [Test]
    public void RootCommand_WithoutSubcommand_KeepsLegacyRenderParsing()
    {
        var root = BuildRootCommand();

        var result = root.Parse(new[] { "--template=file1.txt", "--stdin", "--parser=json" });

        Assert.That(result.Errors, Is.Null.Or.Empty);
    }

    [Test]
    public void ExtensionsRegister_WithReference_ParsesSuccessfully()
    {
        var root = BuildRootCommand();

        var result = root.Parse(new[] { "extensions", "register", "Didot.Expressif" });

        Assert.That(result.Errors, Is.Null.Or.Empty);
    }

    private static Cli.CliRootCommand BuildRootCommand()
    {
        var registerOptions = new Cli.RegisterExtensionOptions();
        var metadataReader = new Cli.ExtensionMetadataReader();
        var resolver = new Cli.ExtensionReferenceResolver(metadataReader, Path.GetTempPath());
        var repository = new Cli.InstallationExtensionRegistryRepository(Path.Combine(Path.GetTempPath(), $"didot-reg-{Guid.NewGuid():N}.json"));
        var handler = new Cli.RegisterExtensionCommandHandler(resolver, metadataReader, repository);
        var registerCommand = new Cli.RegisterExtensionCommand(registerOptions, handler);
        var extensionsCommand = new Cli.ExtensionsCommand(registerCommand);

        return new Cli.CliRootCommand(new Cli.RenderOptions(), extensionsCommand);
    }
}
