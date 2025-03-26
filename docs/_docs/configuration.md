---
title: Template Rendering Configuration
tags: [template, features]
---
## Overview

Didot allows customization of template rendering behavior through the TemplateConfiguration class. One of the key settings available in this configuration is HTML encoding, which determines whether all tags in a template should be automatically encoded to prevent HTML injection and formatting issues.

This configuration is passed to the template engine through a factory, ensuring consistent behavior across multiple templates.

## Passing Configuration to a Template Engine

To apply a configuration, create an instance of TemplateConfiguration and pass it to the FileBasedTemplateEngineFactory when initializing the template engine:

```csharp
var configuration = new TemplateConfiguration(HtmlEncode: true); 
var factory = new FileBasedTemplateEngineFactory(configuration); 
var engine = factory.GetByTag(tag);
```

This ensures that all templates rendered through the engine will adhere to the specified configuration.

## Configuration Settings

### HTML Encoding

The HtmlEncode setting controls whether the template engine should automatically encode special HTML characters (e.g., <, >, &) to prevent unintended rendering issues:

```csharp
public record TemplateConfiguration
(
    bool HtmlEncode = false
);
```

|------|------|
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

Not all template engines support automatic HTML encoding.

The following engines do not support HtmlEncode = true and will ignore this setting:

- DotLiquid
- SmartFormat
- StringTemplate

Other engines, such as **Fluid**, **Scriban**, **Handlebars**, and **Morestachio**, support HTML encoding and will correctly apply the configuration when enabled.
