---
title: Defining a function
tags: [template, features]
---
## Overview

A template function is a function defined inside a template that can render content dynamically. When passed as an argument to another function (like map, each, or custom helpers), it acts as a kind of callback, formatting or transforming values within the template itself.

You define a template block or lambda that takes arguments and renders something with them. Then you pass that function to another function (e.g., a mapping function or loop).

## Registering a Function

To register a function, use the `AddFunction` method of an `ITemplateEngine`. The method requires:

- A name for the function – the identifier for the function.
- A function returning a string representing the function template.

```csharp
var factory = new FileBasedTemplateEngineFactory();
var engine = factory.GetByTag(tag);
engine.AddFunction("Hello", () => "function definition");
```

## Using Functions in Templates

Once registered, functions can be applied directly in templates by referencing the function name and passing values to the parameters. All examples below will have the same output:

```text
Greetings: Mr. Einstein Albert!
```

### Scriban

The main template is defined as,

```liquid
{% raw %}Greetings: {{ Hello(model.Name.First, model.Name.Last) }}!{% endraw %}
```

and the function itself is defined as,

```text
{%- raw -%}
{{ func Hello(firstName, lastName) -}}
  Mr. {{ lastName }} {{ firstName -}}
{{ end }}
{% endraw %}
```

### StringTemplate

The main template is defined as,

```text
{% raw %}Greetings: <Hello(model.Name.First, model.Name.Last)>!{% endraw %}
```

and the function itself is defined as,

```text
{% raw %}Hello(firstName, lastName)::= Mr. <lastName> <firstName>{% endraw %}
```

### Liquid, Fluid, Handlebars, Morestachio and SmartFormat

This feature is not supported.

### Conclusions

Template functions that accept another template function give you a clean, composable way to define how data should be rendered, and reuse that rendering logic flexibly.
