---
title: Mappings of values
tags: [template, features]
---
## Overview

In many cases, you may need to translate a string from one language to another. For example, when converting Pascal to C or C#, you might need to map the Pascal type  `integer` to `int` or `Int32`. **Didot** provides a feature called **mappings**, which allows lookup-based translation using a dictionary.

This feature enables the template engine to perform key-based substitution: when rendering a property, the engine checks a predefined dictionary to find a corresponding value for the given key and outputs the mapped result. To register a dictionary, you need to use the method `AddMappings` from an `ITemplateEngine`. The arguments correspond to the name of the dictionary and all the keys/values defined in a dictionary.

## Registering a Mapping Dictionary

To register a dictionary, use the `AddMappings` method of an `ITemplateEngine`. The method requires:

- A dictionary name – the identifier for the mapping.
- Key-value pairs – defining how values should be translated.

## Using Mappings in Templates

Once registered, mappings can be applied directly in templates by referencing the dictionary name.

```csharp
var factory = new FileBasedTemplateEngineFactory();
var engine = factory.GetByTag(tag);
engine.AddMappings("greetings", new Dictionary() { {"french", "Bonjour"}, {"english", "Hi"}, {"spanish", "Ola"} })
```

Assume the model contains a property *Language*, and a dictionary named *greetings* has been registered to provide translations. The following examples demonstrate how to apply mappings across different template engines:

### Fluid and Scriban

```liquid
{% raw %}Greetings in {{model.Language}} is {{model.Language | greetings}}{% endraw %}
```

### Handlebars

```handlebars
{% raw %}Greetings in {{model.Language}} is {{greetings model.Language}}{% endraw %}
```

### Morestachio

```handlebars
{% raw %}Greetings in {{model.Language}} is {{model.Language.greetings()}}{% endraw %}
```

### StringTemplate

```text
{% raw %}Greetings in <model.Language> is <greetings.(model.Lang)>{% endraw %}
```

### Liquid and SmartFormat

This feature is not supported.

## Conclusion

The mappings feature in Didot simplifies value translation using predefined dictionaries, making it particularly useful for language conversions, type mappings, and localization tasks. By leveraging this feature, templates remain clean, dynamic, and easy to maintain.
