---
title: Basic usage of the CLI
tags: [quick-start, cli-usage]
---
The command to run Didot is simply `didot`. When executing it, you need to provide three required arguments:

- `-t, --Template` (required): Specifies the path to the Scriban, Liquid, Handlebars, StringTemplate or SmartFormat template file.
- `-s, --Source`: Specifies the path to the source data file, which can be in YAML, JSON, or XML format. If this argument is not provided, the data will be read from the console input. In such cases, the `-p, --Parser` option becomes mandatory.
- `-o, --Output`: Specifies the path to the output file where the generated content will be saved. If not provided, the output will be displayed directly in the console.

**Example:**

```bash
didot -t template.scriban -s data.yaml -o page.html
```

In this example:

- `template.scriban` is the Scriban template file.
- `data.yaml` is the source file containing the structured data in YAML format.
- `page.html` is the output file that will contain the generated content.
