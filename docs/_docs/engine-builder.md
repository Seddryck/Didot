---
title: Creating a template engine builder
tags: [template, features]
---
## Overview

The `TemplateEngineBuilder` provides a unified and extensible way to instantiate a template engine of your choice (e.g., Handlebars, Fluid, Scriban, etc.), with optional configuration support.

This enables template rendering abstraction while preserving per-engine flexibility.

## How-to

```csharp
var engine = new TemplateEngineBuilder()
    .UseScriban()
    .Build();

var output = engine.Render(templateSource, model);
```

## Supported Engines

The following engines are available via the builder:

| Engine         | Method                        |
|----------------|-------------------------------|
| Handlebars     | `.UseHandlebars()`            |
| Fluid          | `.UseFluid()`                 |
| Scriban        | `.UseScriban()`               |
| DotLiquid      | `.UseDotLiquid()`             |
| Morestachio    | `.UseMorestachio()`           |
| SmartFormat    | `.UseSmartFormat()`           |
| StringTemplate | `.UseStringTemplate(...)`     |

Each returns the builder instance for fluent chaining.

## Customizing the engine

To further tailor the behavior of certain engines, such as delimiter styles in StringTemplate, the builder supports optional configuration through customization methods.

### Customizing StringTemplate

`StringTemplate` supports configurable delimiters (either `<...>` or `$...$`). Use the overload with a lambda to specify this:

```csharp
// Use angle brackets
var engine = new TemplateEngineBuilder()
    .UseStringTemplate(opt => opt.WithAngleBracketExpressions())
    .Build();

// Or use dollar-sign delimiters
var engine = new TemplateEngineBuilder()
    .UseStringTemplate(opt => opt.WithDollarDelimitedExpressions())
    .Build();
```

By default, StringTemplate uses `<...>` delimiters.

## Global Configuration

To further customize engine behavior, such as enabling HTML encoding, you can attach a [**global configuration**](/docs/engine-configuration) using .`WithConfiguration(...)`. This configuration is shared across all engines and controls cross-cutting concerns — for example, whether the rendered output should be HTML-encoded.

This is different from engine-specific options, which are only applicable to one particular engine (like delimiter styles for StringTemplate).

```csharp
var engine = new TemplateEngineBuilder()
    .UseScriban()
    .WithConfiguration(config => config.WithHtmlEncode())
    .Build();
```

Available options:

| Method         | Description                   |
|----------------|-------------------------------|
| `WithHtmlEncode()` | Enables HTML encoding in output |
| `WithoutHtmlEncode()` | Disables HTML encoding (default) |

The resulting `ITemplateEngine` exposes this configuration via the `Configuration` property.

## Summary

- Use `TemplateEngineBuilder` to select and configure your desired template engine
- Most engines require no options
- `StringTemplate` supports delimiter configuration via an optional builder
- Global behavior (like HTML encoding) can be configured via WithConfiguration(...)
- Once built, the result is an `ITemplateEngine` instance ready for rendering
