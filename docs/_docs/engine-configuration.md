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
var factory = new TemplateEngineFactory();
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

#### Engine Compatibility

Not all engines support automatic HTML encoding. Engines will ignore the HtmlEncode flag if they don’t support it natively.

| Engine | HTML Encoding Support |
|------|------|
| Scriban | ✅ Supported |
| Fluid | ✅ Supported |
| Handlebars | ✅ Supported |
| Morestachio | ✅ Supported |
| DotLiquid | ❌ Ignored |
| SmartFormat | ❌ Ignored |
| StringTemplate | ❌ Ignored |
