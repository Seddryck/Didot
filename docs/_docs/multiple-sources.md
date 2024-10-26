---
title: Using Multiple Sources in Didot
tags: [cli-usage]
---
Didot supports multiple data sources directly from the console, allowing you to build complex models by defining each source as a distinct part. Each source is specified using a key:value format, where each key becomes a unique child within the model structure, allowing you to reference data in a hierarchical way (e.g., model.key).

## Syntax

```bash
--Source=key:path/to/source1.yaml;key2:path/to/source2.json
```

Each key represents a child name under model, allowing you to organize and access data parts with descriptive keys.

## Example usage

Suppose you have two data files that represent different parts of a model:

- `user.yaml`: Contains user information.
- `config.json`: Holds configuration settings.

### Command Example

```powershell
didot -t template.html -s user:user.yaml;settings:config.json -o output.html
```

In this case:

- `user.yaml` is assigned to the `user` key.
- `config.json` is assigned to the `settings` key.

### How Data Appears in the Model

With the above command, the data model created would look like this:

```yaml
model:
  user: # Data from user.yaml
    name: "John Doe"
    email: "john@example.com"
  settings: # Data from settings.json
    theme: "dark"
    notifications: true
```

This allows you to access user and settings data within your template, using paths like model.user.name and model.settings.theme.

### Template usage

The following template is written with **handlebars** template language:
{% raw %}

```handlebars
<h1>User Information</h1>
<p>Name: {{model.user.name}}</p>
<p>Email: {{model.user.email}}</p>

<h2>Settings</h2>
<p>Theme: {{model.settings.theme}}</p>
<p>Notifications Enabled: {{model.settings.notifications}}</p>
```

{% endraw %}

## Key points

- **Format**: Use key:path for each source and separate multiple sources with semicolons.
- **Hierarchy**: Each key becomes a child of model, allowing hierarchical access to each data part in your templates.
- **Flexible**: Data Integration: Combine multiple data sources seamlessly, enabling more complex and modular template generation.
With this approach, Didot provides flexibility in managing multi-source data, making it easy to build robust, organized models for dynamic template generation.