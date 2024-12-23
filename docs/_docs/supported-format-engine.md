---
title: Supported data formats and template engines
tags: [quick-start, cli-usage]
---
**Didot** is a command-line tool designed for generating files based on templating. It supports *YAML*, *JSON*, *FrontMatter/Markdown*, *CSV* and *XML* as source data formats and provides flexibility in templating through both *Scriban*, *Liquid*, *Handlebars*, *StringTemplate*, *SmartFormat* and *Morestachio* templates languages. With Didot, you can easily automate file generation by combining structured data from YAML, JSON, or XML files with customizable templates using Scriban or Liquid.

### Supported Data Formats

- **YAML**: Files with the `.yaml` or `.yml` extension are parsed using a YAML source parser.
- **JSON**: Files with the `.json` extension are parsed using a JSON source parser.
- **XML**: Files with the `.xml` extension are parsed using an XML source parser.
- **URL**: Files with the `.url` extension are parsed using an URL source parser.
- **FrontMatterMarkdown**: Files with the `.md` extension are parsed using YAML parser for the FrontMatter located between two lines of 3 dashes (`---`). The markdown part is added to the key *Content* (overidden any pre-existing value of *Content*).
- **FrontMatter**: Similar to *FrontMatterMarkdown* but doesn't parse the markdown content.

### Supported Templating Engines

Didot utilizes some templating engines, which allow for powerful and flexible templating.

#### Scriban [![GitHub project](https://upload.wikimedia.org/wikipedia/commons/9/91/Octicons-mark-github.svg){: width="16" }](https://github.com/scriban/scriban) [![Documentation]({{ site.baseurl }}/uploads/document-round.png){: width="16" }](https://github.com/scriban/scriban/tree/master/doc)

Templates with the `.scriban` extension are parsed using a Scriban template engine. Scriban is a lightweight and fast template engine with rich support for multiple output formats.

- Highly performant, designed to handle large-scale template processing.
- Supports customizable scripting with rich expressions and filters.
- Can work with JSON and YAML data sources.
- Typical Use Case: Config file generation, reports, email templates, or any templating scenario not tied to a specific web framework.

#### DotLiquid [![GitHub project](https://upload.wikimedia.org/wikipedia/commons/9/91/Octicons-mark-github.svg){: width="16" }](https://github.com/dotliquid/dotliquid) [![Documentation]({{ site.baseurl }}/uploads/document-round.png){: width="16" }](https://github.com/dotliquid/dotliquid/wiki)

Templates with the `.liquid` extension are parsed using a dotLiquid template engine. DotLiquid is a .NET port of the Liquid templating engine used by platforms like Shopify.

- Secure (no access to system objects), making it ideal for user-generated templates.
- Allows both dynamic and static templating.
- Supports filters, tags, and various control flow structures.
- Typical Use Case: SaaS applications, dynamic content rendering, email templates.

#### Fluid [![GitHub project](https://upload.wikimedia.org/wikipedia/commons/9/91/Octicons-mark-github.svg){: width="16" }](https://github.com/sebastienros/fluid) [![Documentation]({{ site.baseurl }}/uploads/document-round.png){: width="16" }](https://github.com/sebastienros/fluid?tab=readme-ov-file#features)

Fully compatible with `.liquid` templates. The Fluid template engine is a fast and secure .NET-based port of the Liquid templating language.

- Optimized for performance, with careful memory management and faster parsing.
- Highly secure (does not expose system objects) and well-suited for user-generated content and environments requiring strict control over input and output.
- Rich templating features, supporting filters, tags, loops, and conditionals.
- Flexible and customizable, making it easy to extend the engine with custom filters or tags.
- Typical Use Case: Applications with complex data bindings, dynamic content generation in websites, CMS platforms, and document templating systems.

#### Handlebars [![GitHub project](https://upload.wikimedia.org/wikipedia/commons/9/91/Octicons-mark-github.svg){: width="16" }](https://github.com/Handlebars-Net/Handlebars.Net) [![Documentation]({{ site.baseurl }}/uploads/document-round.png){: width="16" }](https://handlebarsjs.com/)

Templates with the `.hbs` extension are parsed using a Handlebars template engine. Handlebars C# port of the popular JavaScript Handlebars templating engine.

- Simple syntax for generating HTML or text files from templates.
- Support for helpers, partial templates, and block helpers.
- Good separation of logic from presentation.
- Typical Use Case: Email templates, reports, and content generation.

#### SmartFormat [![GitHub project](https://upload.wikimedia.org/wikipedia/commons/9/91/Octicons-mark-github.svg){: width="16" }](https://github.com/axuno/SmartFormat) [![Documentation]({{ site.baseurl }}/uploads/document-round.png){: width="16" }](https://github.com/axuno/SmartFormat/wiki)

Templates with the `.smart` extension are parsed using a SmartFormat template engine. SmartFormat.Net is a A lightweight templating engine primarily used for string formatting.

- Provides more advanced formatting capabilities than standard string formatting in C#.
- Supports nested templates, conditional formatting, and more.
- Typical Use Case: Log messages, report generation, and dynamic text formatting.

#### StringTemplate [![GitHub project](https://upload.wikimedia.org/wikipedia/commons/9/91/Octicons-mark-github.svg){: width="16" }](https://github.com/kaby76/Domemtech.StringTemplate4) [![Documentation]({{ site.baseurl }}/uploads/document-round.png){: width="16" }](http://www.stringtemplate.org/)

Templates with the `.st` and `.stg` extension are parsed using the StringTemplate engine. StringTemplate is a powerful template engine specifically designed to enforce strict separation of logic from presentation.

- Focused on generating structured text, such as code, XML, and reports.
- Strong emphasis on enforcing Model-View separation.
- Supports conditionals, loops, and automatic escaping to prevent security issues.
- Typical Use Case: Code generation, configuration files, and situations where strict separation between logic and template is required.

#### Morestachio [![GitHub project](https://upload.wikimedia.org/wikipedia/commons/9/91/Octicons-mark-github.svg){: width="16" }](https://github.com/JPVenson/morestachio) [![Documentation]({{ site.baseurl }}/uploads/document-round.png){: width="16" }](https://github.com/JPVenson/morestachio/wiki)

Templates with the `.morestachio` extension are parsed using the Morestachio engine. Morestachio is a lightweight, powerful, flavorful, templating engine for C# and other .net-based languages. Its a fork of Mustachio.

- Simple syntax for generating HTML or text files from templates.
- Mustache adheres to the philosophy of being "logic-less," meaning templates contain no conditionals, loops, or other logic constructs, focusing solely on rendering data passed to them.
- Typical Use Case: Email templates, reports, and content generation.
  