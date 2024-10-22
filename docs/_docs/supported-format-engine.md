---
title: Supported data formats and template engines
tags: [quick-start, cli-usage]
---
**Didot** is a command-line tool designed for generating files based on templating. It supports *YAML*, *JSON*, and *XML* as source data formats and provides flexibility in templating through both *Scriban*, *Liquid*, *Handlebars*, *StringTemplate* and *SmartFormat* templates languages. With Didot, you can easily automate file generation by combining structured data from YAML, JSON, or XML files with customizable templates using Scriban or Liquid.

### Supported Data Formats

- **YAML**: Files with the `.yaml` or `.yml` extension are parsed using a YAML source parser.
- **JSON**: Files with the `.json` extension are parsed using a JSON source parser.
- **XML**: Files with the `.xml` extension are parsed using an XML source parser.

### Supported Templating Engines

Didot utilizes some templating engines, which allow for powerful and flexible templating.

- **Scriban**: Templates with the `.scriban` extension are parsed using a Scriban template engine. Scriban is a lightweight and fast template engine with rich support for multiple output formats.
  - Highly performant, designed to handle large-scale template processing.
  - Supports customizable scripting with rich expressions and filters.
  - Can work with JSON and YAML data sources.
  - Typical Use Case: Config file generation, reports, email templates, or any templating scenario not tied to a specific web framework.
- **Liquid**: Templates with the `.liquid` extension are parsed using a dotLiquid template engine. DotLiquid is a .NET port of the Liquid templating engine used by platforms like Shopify.
  - Secure (no access to system objects), making it ideal for user-generated templates.
  - Allows both dynamic and static templating.
  - Supports filters, tags, and various control flow structures.
  - Typical Use Case: SaaS applications, dynamic content rendering, email templates.
- **Handlebars**: Templates with the `.hbs` extension are parsed using a Handlebars template engine. Handlebars C# port of the popular JavaScript Handlebars templating engine.
  - Simple syntax for generating HTML or text files from templates.
  - Support for helpers, partial templates, and block helpers.
  - Good separation of logic from presentation.
  - Typical Use Case: Email templates, reports, and content generation.
- **SmartFormat**: Templates with the `.smart` extension are parsed using a SmartFormat template engine. SmartFormat.Net is a A lightweight templating engine primarily used for string formatting.
  - Provides more advanced formatting capabilities than standard string formatting in C#.
  - Supports nested templates, conditional formatting, and more.
  - Typical Use Case: Log messages, report generation, and dynamic text formatting.
- **StringTemplate**: Templates with the `.st` and `.stg` extension are parsed using the StringTemplate engine. StringTemplate is a powerful template engine specifically designed to enforce strict separation of logic from presentation.
  - Focused on generating structured text, such as code, XML, and reports.
  - Strong emphasis on enforcing Model-View separation.
  - Supports conditionals, loops, and automatic escaping to prevent security issues.
  - Typical Use Case: Code generation, configuration files, and situations where strict separation between logic and template is required.