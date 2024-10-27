---
title: Usage of StdIn and StdOut
tags: [cli-usage]
---
## With data from the console

- `-i, --stdin`: Specifies that the input is coming from the console. This option is required only when the --Source argument is omitted.
- `-r, --parser`: Defines the parser to use when the source data is provided through the console. Accepted values are `yaml`, `json` or `xml`. This option is required only when the `--source` argument is omitted or if the extension of the source file is not recognized to determine the parser.

<sub>CMD:</sub>
```bash
type "data.json" | didot --stdin -t template.hbs -r json -o output.txt
```

<sub>PowerShell:</sub>
```powershell
Get-Content data.json | didot --stdin -t template.hbs -r json -o output.txt
```

<sub>Bash:</sub>
```bash
cat data.json | didot --stdin -t template.hbs -r json -o output.txt
```

In this example:

- `template.hbs` is the Handlebars template file.
- `json` is the parser of input data.
- the output is redirected to the console.

## With content rendered on the console

Simply omit the `-o` option and the output will be rendered on the console.

<sub>PowerShell:</sub>
```powershell
didot --stdin -t template.hbs -r json
```
