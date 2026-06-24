using Didot.Core;

namespace Didot.Cli.Testing.Extensions;

[DidotExtension("didot.cli.testing.extension", "Cli Testing Hook")]
public sealed class PipelineHookForE2eTests : IPipelineExtensionHook
{
    public object Apply(object model)
    {
        if (!TryReadModel(model, out var firstName, out var lastName))
            return model;

        return new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            ["fullname"] = $"{firstName} {lastName}".Trim(),
        };
    }

    private static bool TryReadModel(object model, out string firstName, out string lastName)
    {
        firstName = string.Empty;
        lastName = string.Empty;

        if (model is IDictionary<string, object?> nullableDictionary)
            return TryGetValue(nullableDictionary, "firstname", out firstName)
                && TryGetValue(nullableDictionary, "lastname", out lastName);

        if (model is System.Collections.IDictionary legacyDictionary)
            return TryGetValue(legacyDictionary, "firstname", out firstName)
                && TryGetValue(legacyDictionary, "lastname", out lastName);

        return false;
    }

    private static bool TryGetValue(IDictionary<string, object?> dictionary, string key, out string value)
    {
        value = string.Empty;

        if (!dictionary.TryGetValue(key, out var item)
            && !dictionary.TryGetValue(key.ToLowerInvariant(), out item)
            && !dictionary.TryGetValue(key.ToUpperInvariant(), out item)
            && !dictionary.TryGetValue(char.ToUpperInvariant(key[0]) + key[1..], out item)
            && !dictionary.TryGetValue(char.ToLowerInvariant(key[0]) + key[1..], out item))
        {
            return false;
        }

        value = item?.ToString() ?? string.Empty;
        return !string.IsNullOrWhiteSpace(value);
    }

    private static bool TryGetValue(System.Collections.IDictionary dictionary, string key, out string value)
    {
        value = string.Empty;
        if (!dictionary.Contains(key)
            && !dictionary.Contains(key.ToLowerInvariant())
            && !dictionary.Contains(key.ToUpperInvariant())
            && !dictionary.Contains(char.ToUpperInvariant(key[0]) + key[1..])
            && !dictionary.Contains(char.ToLowerInvariant(key[0]) + key[1..]))
        {
            return false;
        }

        var item = dictionary[key]
            ?? dictionary[key.ToLowerInvariant()]
            ?? dictionary[key.ToUpperInvariant()]
            ?? dictionary[char.ToUpperInvariant(key[0]) + key[1..]]
            ?? dictionary[char.ToLowerInvariant(key[0]) + key[1..]];

        value = item?.ToString() ?? string.Empty;
        return !string.IsNullOrWhiteSpace(value);
    }
}

[DidotExtension("didot.cli.testing.throwing", "Cli Testing Throwing Hook")]
public sealed class ThrowingPipelineHookForE2eTests : IPipelineExtensionHook
{
    public object Apply(object model)
        => throw new InvalidOperationException("hook was applied");
}
