---
title: Specify template engine or parser
tags: [cli-usage]
---
## Specify the data parser

- `-p, --Parser`: Defines the parser to use when the source data is provided through the console. Accepted values are `yaml`, `json` or `xml`. This option is required only when the `--Source` argument is omitted or if the extension of the source file is not recognized to determine the parser.

```powershell
didot -t template.hbs -s data.txt -p json -o output.txt
```

In this example:

- `template.hbs` is the Handlebars template file.
- `data.txt` is the source file.
- `json` is the parser of input data from the source file.
- `output.txt` is the file where the output will be rendered.

## Specify the template engine

### Direct specification

- `-e, --Engine`: Defines the template engine to use independantly of the template file extension. Accepted values are `scriban`, `dotliquid`, `fluid`, `handlebars`, `stringtemplate`, `smartformat`.

```powershell
didot -t template.txt -s data.json -e handlebars -o output.txt
```

In this example:

- `template.txt` is the template file.
- `data.json` is the data JSON file.
- `handlebars` is the template engine to use.
- `output.txt` is the file where the output will be rendered.

### Add or replace extension associations

- `-x, --Extensions`: Defines the association of a file's extension with a template engine. More than one can be specified.

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
