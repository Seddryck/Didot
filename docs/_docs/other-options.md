---
title: Other options
tags: [cli-usage]
---
## With data from the console

- `-i, --StdIn`: Specifies that the input is coming from the console. This option is required only when the --Source argument is omitted.
- `-p, --Parser`: Defines the parser to use when the source data is provided through the console. Accepted values are `yaml`, `json` or `xml`. This option is required only when the `--Source` argument is omitted or if the extension of the source file is not recognized to determine the parser.

<sub>CMD:</sub>
```cmd
type data.json | didot --StdIn -t template.hbs -p json -o output.txt
```

<sub>PowerShell:</sub>
```powershell
Get-Content data.json | didot --StdIn -t template.hbs -p json -o output.txt
```

<sub>Bash:</sub>
```bash
cat data.json | didot --StdIn -t template.hbs -p json -o output.txt
```

In this example:

* `template.hbs` is the Handlebars template file.
* `json` is the parser of input data.
* the output is redirected to the console.

## With content rendered on the console

Simply omit the `-o` option and the output will be rendered on the console.

<sub>PowerShell:</sub>
```powershell
didot --StdIn -t template.hbs -p json
```
