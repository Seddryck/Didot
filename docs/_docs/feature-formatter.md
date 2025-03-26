---
title: Formatters of Values
tags: [template, features]
---
## Overview

In many situations, you may need to transform a value before rendering it in a template. For example, you might want to format **dates, numbers, or text** according to specific rules. **Didot** provides a feature called formatters, which allows you to apply transformations to values using predefined formatting functions.

This feature enables the template engine to **modify the output of a property dynamically—for instance**, converting a date into a specific format or rounding a number to a given precision.

## Registering a Formatter

To register a custom formatter, use the `AddFormatter` method of an `ITemplateEngine`. This method requires:

- A formatter name – an identifier for the function.
- A function delegate – defining how the value should be transformed.

Once registered, formatters can be applied to any property within the template.

## Using Formatters in Templates

Once registered, a formatter can be applied to values in templates by referencing the formatter name.

```csharp
var factory = new FileBasedTemplateEngineFactory();
var engine = factory.GetByTag(tag);
engine.AddMappings("currency", (object value) => $"{value}€");
```

Assume the model contains a property *Price*, and a formatter named *currency* has been registered to format numbers as currency. The following examples demonstrate how to apply formatters across different template engines:

### Fluid and Scriban

```liquid
{% raw %}The price is {{ model.Price | currency }}{% endraw %}
```

### Handlebars

```handlebars
{% raw %}The price is {{ currency model.Price }}{% endraw %}
```

### Morestachio

```handlebars
{% raw %}The price is {{ model.Price.currency() }}{% endraw %}
```

### StringTemplate

```text
The price is <model.Price; format=\"currency\">
```

### Liquid and SmartFormat

This feature is not supported.

## Conclusion

The formatters feature in **Didot** allows you to apply transformations dynamically to values before rendering, making it particularly useful for date formatting, number formatting, text transformations, and other custom processing needs. By leveraging this feature, templates remain clean, flexible, and reusable.
