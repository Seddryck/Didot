---
title: Template Rendering Configuration
tags: [template, features]
---
## Overview

Didot allows you to customize template rendering behavior using the `TemplateConfiguration` class. This configuration ensures consistent behavior across multiple templates, such as whether special characters should be HTML-encoded to prevent injection or formatting issues.

This configuration can be applied globally via the `TemplateEngineFactory` or passed directly to other components that support rendering logic.

## Applying Configuration via Factory

The recommended way to apply configuration is through the [`TemplateEngineFactory`](/docs/engine-factory). This approach ensures that all engines built by the factory share the same rendering behavior:

```csharp
var factory = TemplateEngineFactory.Default();
factory.Configure(config => config.WithHtmlEncode());

var engine = factory.Create(".scriban");
```

All engines created afterward (e.g., .hbs, .morestachio, etc.) will apply this configuration unless overridden.

## Passing Configuration directly to a Template Engine

You can also instantiate a configuration and pass it manually if you're using a `FileBasedTemplateEngineFactory`:

```csharp
var configuration = new TemplateConfiguration(HtmlEncode: true); 
var factory = new FileBasedTemplateEngineFactory(configuration); 
var engine = factory.GetByTag(tag);
```

This approach applies only to file-based resolution logic.

## Configuration Settings

### HTML Encoding

The `HtmlEncode` setting controls whether the template engine should automatically encode special HTML characters (e.g., `<`, `>`, `&`) to prevent unintended rendering issues:

```csharp
public record TemplateConfiguration
(
    bool HtmlEncode = false
);
```

| Setting | Behaviour |
|------|------|
| `HtmlEncode = true` | All rendered tags will be HTML-encoded (e.g., `<` → `&lt;`) |
| `HtmlEncode = false` | Content is rendered as is, without encoding |

#### Example

If HtmlEncode = true and the model contains:

```csharp
new { Message = "<b>Hello</b>" }
```

Then the output will be:

```html
&lt;b&gt;Hello&lt;/b&gt;
```

If HtmlEncode = false, the output remains:

```html
<b>Hello</b>
```

### Wrap as Model

The `WrapAsModel` setting controls whether the template engine should wrap the provided model inside a root property named `model` when rendering.

This allows a consistent access pattern like `{{ model.Name }}` or `{{ model.Age }}` even when passing multiple loose values.

| Setting | Behaviour |
|------|------|
| `WrapAsModel = true` (default) | Input is wrapped under a model key (e.g., model.Name) |
| `WrapAsModel = false` | Values are injected directly into the root context (e.g., Name) |

**Example**: Given this model,

```csharp
new Dictionary<string, object>
{
    ["Name"] = "Albert",
    ["Age"] = 42
}
```

With WrapAsModel = true, templates should access data like:

```handlebars
Hello {{ model.Name }} — Age: {{ model.Age }}
```

With WrapAsModel = false, templates can use:

```handlebars
Hello {{ Name }} — Age: {{ Age }}
```

This allows greater flexibility when writing templates, especially when targeting engines that support root-scope variables.

#### Note on Double-Wrapping Prevention

When `WrapAsModel = true`, Didot checks whether the object being passed to the renderer already contains a property named "model". If this property exists, Didot assumes the object is already wrapped and skips wrapping it again.

This ensures compatibility and prevents double-wrapping scenarios that would otherwise lead to awkward access patterns like `model.model.Name`.

#### Configuring via Builder

Use TemplateConfigurationBuilder to fluently define configuration settings:

```csharp
var config = new TemplateConfigurationBuilder()
    .WithHtmlEncode()
    .WithoutWrapAsModel()
    .Build();
```

This builder is typically passed to `TemplateEngineBuilder.WithConfiguration(...)` or `TemplateEngineFactory.Configure(...)`.

#### Engine Compatibility

| Engine | Wrap as Model |
|------|------|
| Scriban | ✅ Supported |
| Fluid | ✅ Supported |
| Handlebars | ✅ Supported |
| Morestachio | ✅ Supported |
| DotLiquid | ✅ Supported |
| SmartFormat | ✅ Supported |
| StringTemplate | ✅ Supported |
