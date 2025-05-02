---
title: Defining a partial
tags: [template, features]
---
## Overview

A partial is a reusable snippet of template code that can be included or embedded within one or more templates. It allows you to extract repeated layout or formatting logic into a separate, named unit, improving both readability and maintainability.

When a template includes a partial, the templating engine:

1. Looks up the partial by its name
2. Loads or renders its content
3. Injects the rendered result into the main template at the include point

## Registering a Partial

To register a partial, use the `AddPartial` method of an `ITemplateEngine`. The method requires:

- A name for the partial – the identifier for the partial.
- A function returning a string representing the partial template.

## Using Partials in Templates

Once registered, mappings can be applied directly in templates by referencing the dictionary name.

```csharp
var factory = new FileBasedTemplateEngineFactory();
var engine = factory.GetByTag(tag);
engine.AddPartial("greetings", () => "Welcome");
```

In this case, at each occurrence of the partial *greetings*, it will output the text *Welcome*.

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

### Liquid, Fluid, Morestachio and SmartFormat

This feature is not supported.
