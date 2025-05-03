---
title: Defining a partial
tags: [template, features]
---
## Overview

A partial is a reusable snippet of template code that can be included or embedded within one or more templates. It allows you to extract repeated layout or formatting logic into a separate, named unit, improving both readability and maintainability.

When a template includes a partial, the templating engine:

1. Looks up the partial by its name
2. Loads or renders its content
3. Injects the rendered result into the main template at the point of inclusion

## Registering a Partial

To register a partial, use the `AddPartial` method of an `ITemplateEngine`. The method requires:

- A name for the partial – the identifier for the partial.
- A function returning a string representing the partial template.

## Using Partials in Templates

Once registered, mappings can be applied directly in templates by referencing the dictionary name.

```csharp
var factory = TemplateEngineFactory.Default;
var engine = factory.Create(".scriban");
engine.AddPartial("greetings", () => "Welcome");
```

In this case, at each occurrence of the partial *greetings*, it will output the text *Welcome*.

```text
Welcome, Albert Einstein!
```

### Scriban

```liquid
{% raw %}{{include 'greetings'}}, {{model.Name.First}} {{model.Name.Last}}!{% endraw %}
```

### Handlebars

```handlebars
{% raw %}{{> Greetings }}, {{model.Name.First}} {{model.Name.Last}}!{% endraw %}
```

### StringTemplate

```text
{% raw %}<Greetings()>, <model.Name.First> <model.Name.Last>!{% endraw %}
```

## Engine Compatibility

Not all engines support automatic partial templates. Engines will throw a `NotSupportedException` natively.

| Engine | Partial Support |
|------|------|
| Scriban | ✅ Supported |
| Fluid | ❌ Throws |
| Handlebars | ✅ Supported |
| Morestachio | ❌ Throws |
| DotLiquid | ❌ Throws |
| SmartFormat | ❌ Throws |
| StringTemplate | ✅ Supported |
