---
title: Cached Template Rendering
tags: [template, features]
---
## Overview

The `IRenderer` interface defines a contract for rendering a template **with a model**, while **reusing the expensive setup parts** of the rendering process, such as template parsing and context binding.

It enables you to prepare a template **once**, bind the rendering context, and render it multiple times efficiently with different data models.

```csharp
public interface IRenderer
{
    string Render(object model);
}
```

Each `ITemplateEngine` implementation builds an `IRenderer` instance that:

- Compiles or parses the template once
- Initializes engine-specific context only once (if applicable)
- Reuses the compiled representation for multiple `Render(...)` calls

This reduces runtime overhead and improves performance in scenarios like batch processing or repeated rendering of the same structure.

### Why Use Cached Rendering?

| Aspect                | `ITemplateEngine.Render(...)`      | `IRenderer.Render(...)`                 |
|-----------------------|-------------------------------------|------------------------------------------|
| **Performance**       | Re-compiles template and context every time | ✅ Caches parsed/compiled templates        |
| **Context Reuse**     | Rebuilds context per call            | ✅ Caches context creation logic (where supported) |
| **Efficiency**        | Suitable for one-off rendering       | ✅ Optimized for many renders with same template |
| **Memory**            | Reallocates internals each call      | ✅ Shares compiled structures where supported |

## Render Pipeline: What Gets Cached?

The rendering process involves three core elements:

1. **Compiled Template**: The parsed, compiled version of the template source string.
2. **Rendering Context**: Template-specific state (partials, formatters, mappings).
3. **Model Binding**: The actual input data used at render time.

Here's a breakdown of what is cached and what is created per call for each template engine:

| Engine         | Compiled Template ✅ | Context Reuse 🔄                | Notes                                                                 |
|----------------|----------------------|----------------------------------|-----------------------------------------------------------------------|
| **Scriban**       | ✅ `Template.Parse()`        | ✅ via `TemplateContext` factory | Full reuse of compiled template and shared context strategy          |
| **SmartFormat**   | ❌ (uses plain string)       | ❌ none                         | No pre-compilation or context reuse                                  |
| **StringTemplate**| ✅ `CompiledTemplate`        | ✅ via `TemplateGroup`          | Group holds shared templates and compiled representation             |
| **Morestachio**   | ✅ Renderer and AST cached    | ✅ via `IRenderer`              | Parser + compiled renderer retained                                  |
| **Handlebars**    | ✅ Compiled delegate          | ✅ if `IHandlebars` reused      | Parser + compiled renderer       |
| **Fluid**         | ✅ `IFluidTemplate`           | ✅ `TemplateContext` per model  | Efficient reuse with pre-compiled structure                          |
| **DotLiquid**     | ✅ `Template.Parse()`         | ❌ `Hash` is recreated          | Context is built fresh each time unless you reuse `Hash` manually    |

> ✅ **Compiled Template**: Caching avoids re-parsing the same template string  
> 🔄 **Context Reuse**: Shared context (partials, helpers) setup — varies by engine  

**Model Binding**, always occurs per render call — this is never cached.

## Example Usage

If you plan to render the same template multiple times (with different models), **use `Prepare(...)` from an `ITemplatEngine` to get an `IRenderer`**, then call `.Render(model)` on it. This avoids redundant parsing, compiling, or binding overhead.

```csharp
// Prepare once
var factory = TemplateEngineFactory.Default;
var engine = factory.Create(".scriban");
var renderer = engine.Prepare("Hello {{ model.Name }}");

// Render many times
var result1 = renderer.Render(new { Name = "Alice" });
var result2 = renderer.Render(new { Name = "Bob" });
```

## Summary

- `IRenderer` is a high-performance rendering abstraction designed for **template reuse**
- It avoids re-parsing or re-binding context on every render
- Ideal when rendering the same template repeatedly with different data
- Backed by engine-specific optimizations (e.g., compiled Scriban templates, Fluid parsing, StringTemplate groups)
