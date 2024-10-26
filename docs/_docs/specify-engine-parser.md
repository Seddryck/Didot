---
title: Specify template engine or parser
tags: [cli-usage]
---
## Specify the data parser

### Direct specification of data parser

- `-p, --Parser`: Defines the parser to use when the source data is provided through the console. Accepted values are `yaml`, `json`, `xml`, `FrontMatter` or `FrontMatterMarkdown`. This option is required only when the `--Source` argument is omitted or if the extension of the source file is not recognized to determine the parser. For [multiple sources](../multiple-sources), this option applies to all files, regardless of their extensions or any pre-assigned engine associations.

```bash
didot -t template.hbs -s data.txt -p json -o output.txt
```

In this example:

- `template.hbs` is the Handlebars template file.
- `data.txt` is the source file.
- `json` is the parser of input data from the source file.
- `output.txt` is the file where the output will be rendered.

### Add or replace extension associations for data parsers

- `-X, --ParserExtension`: Defines the association of a file's extension with a parser. More than one association can be specified.

```bash
didot -t template.txt -s data.json -X dat:Json;fm:FrontMatter -o output.txt
```

In this example:

- `template.txt` is the template file.
- `data.json` is the data JSON file.
- `dat:Json;fm:FrontMatter` is associating JSON to the extension `.dat` and FrontMatter to the extension `.fm`.
- `output.txt` is the file where the output will be rendered.

By default following file's extension association are registered:

- `.json` to JSON
- `.yaml` to YAML
- `.yml` to YAML
- `.xml` to XML
- `.md` to FrontMatterMarkdown

## Specify the template engine

### Direct specification of template engine

- `-e, --Engine`: Defines the template engine to use independantly of the template file extension. Accepted values are `scriban`, `dotliquid`, `fluid`, `handlebars`, `stringtemplate`, `smartformat`.

```powershell
didot -t template.txt -s data.json -e handlebars -o output.txt
```

In this example:

- `template.txt` is the template file.
- `data.json` is the data JSON file.
- `handlebars` is the template engine to use.
- `output.txt` is the file where the output will be rendered.

### Add or replace extension associations for template engines

- `-x, --EngineExtension`: Defines the association of a file's extension with a template engine. More than one can be specified.

```powershell
didot -t template.txt -s data.json -x txt:handlebars;liquid:fluid -o output.txt
```

In this example:

- `template.txt` is the template file.
- `data.json` is the data JSON file.
- `txt:handlebars;liquid:fluid` is associating Handlbars to the extension `.txt` and Fluid to the extension `.liquid`.
- `output.txt` is the file where the output will be rendered.

By default following file's extension association are registered:

- `.scriban` to Scriban
- `.liquid` to DotLiquid
- `.hbs` to Handlebars
- `.smart` to SmartFormat
- `.st` and `.stg` to StringTemplate
