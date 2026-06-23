using System.Reflection;
using Didot.Core;
using NUnit.Framework;

namespace Didot.Cli.Testing.Extensions;

public class ExtensionRegistrationServicesTests
{
    [Test]
    public void MetadataReader_WithoutVersionInAttribute_FallsBackToAssemblyVersion()
    {
        var reader = new Cli.ExtensionMetadataReader();
        var assemblyPath = Assembly.GetExecutingAssembly().Location;

        var metadata = reader.Read(assemblyPath);

        Assert.That(metadata.Id, Is.EqualTo("didot.cli.testing.extension"));
        Assert.That(metadata.Name, Is.EqualTo("Cli Testing Extension"));
        Assert.That(metadata.Version, Is.EqualTo(Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0.0"));
    }

    [Test]
    public void ReferenceResolver_ExplicitDllPath_ReturnsAbsolutePath()
    {
        var reader = new Cli.ExtensionMetadataReader();
        var resolver = new Cli.ExtensionReferenceResolver(reader, Path.GetTempPath());

        var sourceAssembly = Assembly.GetExecutingAssembly().Location;
        var tempDirectory = Path.Combine(Path.GetTempPath(), $"didot-ext-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDirectory);

        try
        {
            var destination = Path.Combine(tempDirectory, "Didot.Extension.dll");
            File.Copy(sourceAssembly, destination, true);

            var resolved = resolver.ResolveAssemblyPath(destination);

            Assert.That(resolved, Is.EqualTo(Path.GetFullPath(destination)));
        }
        finally
        {
            if (Directory.Exists(tempDirectory))
                Directory.Delete(tempDirectory, true);
        }
    }

    [Test]
    public void ReferenceResolver_NonPathWithMultipleMatches_Throws()
    {
        var sourceAssembly = Assembly.GetExecutingAssembly().Location;
        var rootDirectory = Path.Combine(Path.GetTempPath(), $"didot-ext-test-{Guid.NewGuid():N}");
        var extensionsDirectory = Path.Combine(rootDirectory, "extensions");
        Directory.CreateDirectory(extensionsDirectory);

        var originalCurrentDirectory = Directory.GetCurrentDirectory();
        try
        {
            File.Copy(sourceAssembly, Path.Combine(rootDirectory, "A.dll"), true);
            File.Copy(sourceAssembly, Path.Combine(extensionsDirectory, "B.dll"), true);

            Directory.SetCurrentDirectory(rootDirectory);
            var reader = new Cli.ExtensionMetadataReader();
            var resolver = new Cli.ExtensionReferenceResolver(reader, Path.Combine(rootDirectory, "install"));

            Assert.That(
                () => resolver.ResolveAssemblyPath("didot.cli.testing.extension"),
                Throws.TypeOf<InvalidOperationException>().With.Message.Contains("Multiple extensions match")
            );
        }
        finally
        {
            Directory.SetCurrentDirectory(originalCurrentDirectory);
            if (Directory.Exists(rootDirectory))
                Directory.Delete(rootDirectory, true);
        }
    }

    [Test]
    public void InstallationRegistryRepository_RegisterAndRead_Success()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"didot-ext-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);

        var registryPath = Path.Combine(directory, "didot.extensions.registry.json");
        var repository = new Cli.InstallationExtensionRegistryRepository(registryPath);

        try
        {
            repository.Register(new Cli.ExtensionRegistryEntry
            {
                Id = "didot.expressif",
                Name = "Expressif",
                Assembly = @"C:\\extensions\\Didot.Expressif.dll",
                Enabled = true,
                Version = "1.0.0",
                RegisteredAt = DateTimeOffset.UtcNow,
            });

            var entries = repository.ReadAll();
            Assert.That(entries, Has.Count.EqualTo(1));
            Assert.That(entries[0].Id, Is.EqualTo("didot.expressif"));
        }
        finally
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
        }
    }

    [Test]
    public void InstallationRegistryRepository_RegisterDuplicateId_Throws()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"didot-ext-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);

        var registryPath = Path.Combine(directory, "didot.extensions.registry.json");
        var repository = new Cli.InstallationExtensionRegistryRepository(registryPath);

        try
        {
            var first = new Cli.ExtensionRegistryEntry
            {
                Id = "didot.expressif",
                Name = "Expressif",
                Assembly = @"C:\extensions\Didot.Expressif.dll",
                Enabled = true,
                Version = "1.0.0",
                RegisteredAt = DateTimeOffset.UtcNow,
            };

            var duplicateId = new Cli.ExtensionRegistryEntry
            {
                Id = "didot.expressif",
                Name = "Expressif 2",
                Assembly = @"C:\extensions\Didot.Expressif2.dll",
                Enabled = true,
                Version = "1.0.1",
                RegisteredAt = DateTimeOffset.UtcNow,
            };

            repository.Register(first);

            Assert.That(
                () => repository.Register(duplicateId),
                Throws.TypeOf<InvalidOperationException>().With.Message.Contains("already registered")
            );
        }
        finally
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
        }
    }

    [Test]
    public void MetadataReader_WhenAttributeMissing_ReturnsError()
    {
        var reader = new Cli.ExtensionMetadataReader();
        var assemblyPath = typeof(Cli.Program).Assembly.Location;

        var success = reader.TryRead(assemblyPath, out var metadata, out var error);

        Assert.That(success, Is.False);
        Assert.That(metadata, Is.Null);
        Assert.That(error, Does.Contain("DidotExtensionAttribute"));
    }

    [Test]
    public void ExtensionAssemblyLoader_Load_WithCompatibleType_Success()
    {
        var loader = new Cli.ExtensionAssemblyLoader();
        var assemblyPath = Assembly.GetExecutingAssembly().Location;

        var extension = loader.Load(assemblyPath, "didot.cli.testing.extension");

        Assert.That(extension, Is.Not.Null);
        Assert.That(extension.Instance, Is.TypeOf<PipelineHookForE2eTests>());
    }

    [Test]
    public void ExtensionAssemblyLoader_Load_WithoutCompatibleType_Failure()
    {
        var loader = new Cli.ExtensionAssemblyLoader();
        var assemblyPath = typeof(Cli.Program).Assembly.Location;

        Assert.That(
            () => loader.Load(assemblyPath),
            Throws.TypeOf<ExtensionTypeNotFoundException>());
    }

    [Test]
    public void RenderCommandHandler_LoadExtensionHooks_LoadsRegisteredHook()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"didot-ext-test-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        var registryPath = Path.Combine(directory, Cli.InstallationExtensionRegistryRepository.RegistryFileName);

        try
        {
            var repository = new Cli.InstallationExtensionRegistryRepository(registryPath);
            repository.Register(new Cli.ExtensionRegistryEntry
            {
                Id = "didot.cli.testing.extension",
                Name = "Pipeline hook",
                Assembly = Assembly.GetExecutingAssembly().Location,
                Enabled = true,
                Version = "1.0.0",
                RegisteredAt = DateTimeOffset.UtcNow,
            });

            var resolver = new Cli.InstallationExtensionSourceResolver(repository, directory);
            var loader = new Cli.ExtensionAssemblyLoader();
            var handler = new RenderCommandHandlerProbe(resolver, loader);

            var hooks = handler.InvokeLoadExtensionHooks();

            Assert.That(hooks, Has.Count.EqualTo(1));
            Assert.That(hooks[0], Is.TypeOf<PipelineHookForE2eTests>());
        }
        finally
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, true);
        }
    }

    private sealed class RenderCommandHandlerProbe(
        Cli.InstallationExtensionSourceResolver resolver,
        Cli.ExtensionAssemblyLoader loader)
        : Cli.RenderCommandHandler(null, resolver, loader)
    {
        public IList<IPipelineExtensionHook> InvokeLoadExtensionHooks()
            => LoadExtensionHooks();
    }
}
