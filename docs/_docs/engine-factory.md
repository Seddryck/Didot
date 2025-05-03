---
title: Instantiating a template engine
tags: [template, features]
---
## Overview

The `TemplateEngineFactory` manages mappings between file extensions and templating engine builders. It allows registering, overriding, and instantiating engines dynamically — with optional **global configuration** applied at creation time.

## Default instance

A default instance is accessible via:

```csharp
TemplateEngineFactory.Default
```

It comes preconfigured with these mappings:

| Extension        | Engine                  |
|------------------|--------------------------|
| `.liquid`        | DotLiquid                |
| `.hbs`           | Handlebars               |
| `.st`, `.stg`    | StringTemplate           |
| `.morestachio`   | Morestachio              |
| `.mustache`      | Morestachio              |
| `.smart`         | SmartFormat              |
| `.scriban`       | Scriban                  |

---

## Registering Engines

To support different file types or template syntaxes, you can register template engines by associating them with file extensions. This allows the factory to automatically select the correct engine based on a template's file extension.

### Register a builder directly

```csharp
factory.AddOrReplace(".foo", new TemplateEngineBuilder().UseScriban());
```

### Register a builder with a lambda

```csharp
factory.AddOrReplace(".foo", builder => builder.UseScriban());
```

## Creating Engines

To create (instantiate) a configured engine based on file extension:

```csharp
var engine = factory.Create(".scriban");
```

If no matching extension is found, a `NotSupportedException` is thrown.

---

## Global Configuration

You can define a [**global configuration**](/docs/engine-configuration) that will be applied to **all engine instances** created after it is set.

```csharp
factory.Configure(config => config.WithHtmlEncode());
```

This affects any subsequent call to `.Create(...)`:

```csharp
var engine = factory.Create(".scriban");
// engine.Configuration.HtmlEncode will now return `true` based on configuration
```

Unlike engine-specific options (like delimiters for StringTemplate), this configuration is **shared** across all engines and typically affects runtime behavior (e.g., encoding).

## Checking and Managing Extensions

To inspect or modify the factory's registered extensions, a set of utility methods is available. These allow you to check if an extension is supported, list all registered types, or remove or clear mappings as needed.

### Check existence

```csharp
bool exists = factory.Exists(".hbs");
```

### Remove

```csharp
factory.Remove(".hbs");
```

### Clear all

```csharp
factory.Clear();
```

### List supported extensions

```csharp
string[] all = factory.AllSupportedExtensions();
```

### Count total registrations

```csharp
int count = factory.Count;
```

## Extension Normalization

- All extensions are normalized to start with a dot (`.`).
- `"stg"` is treated the same as `".stg"`.

## Summary

- `TemplateEngineFactory` maps extensions to template engine builders.
- `Create()` builds the appropriate engine, optionally applying a shared configuration.
- `Configure()` lets you define a common behavior like HTML encoding across all engines.
- Useful for templating systems supporting multiple syntaxes without hardcoding logic.
