---
title: CLI options
tags: [quick-start, cli-usage]
---
## General format for options

### Shortcut Option (Single Character)

Shortcut options are prefixed with a single hyphen (-) and can be followed directly by the value without a space, though a space can be used if preferred.
**Example:**

- Without space: `-tpath/to/template`
- With space: `-t path/to/template`

### Long Version Option (Double Dash)

Long options are prefixed with double hyphens (--), the first letter of the option is uppercase, and the option name is followed by an equal sign (=) to assign its value.
**Example:** `--Template=path/to/template`

## Option Values

### Single Value

An option can take a single value, which is provided right after the shortcut or the `=` sign for long options.
**Example**: `-s path/to/source` or `--Source=path/to/source`

### Switch Values

If an option accepts a boolean, this one can be omitted.
**Example**: `-i` or `--StdIn` or `--StdIn=true`

### Multiple Values

If an option accepts multiple values, the values should be separated by a semicolon (;).
**Example**: `--Sources=file1.yaml;file2.json;file3.xml`

### Key-Value Pairs

For key-value pairs, the key is followed by a colon (:), and then the value. Multiple key-value pairs, if allowed, should be separated by a semicolon (;).
**Example**: `--ParserExtension=txt:handlebars;liquid:fluid`

## Didot Options Explained

### Template Option

Shortcut: `-t`
Long: `--Template`
Description: Specifies the path to the template file.
Accept: single value.
Mandatory: yes.
Example: `-tpath/to/template` or `--Template=path/to/template`

### Engine option

Shortcut: -e
Long: --Engine
Description: Specifies the template engine to use (scriban, fluid, dotliquid, handlebars, smartformat, stringtemplate).
Accept: single value. When omitted Didot will select the engine based on the extension of the template file.
Example: `-efluid` or `--Engine=fluid`

### Engine files' extension association option

Shortcut: -x
Long: --EngineExtension
Description: Specifies additional or replacing association between a file extension and an engine for automatic detection
Accept: multiple key-value pairs.
Mandatory: no.
Example: `-xtxt:handlebars;liquid:fluid` or `--EngineExtension=txt:handlebars;liquid:fluid`

### Source Option

Shortcut: `-s`
Long: `--Source`
Description: Specifies the path to the source file. If omitted, input can be taken from StdIn.
Accept: single value.
Exclusive: can't be set with the parameter `--StdIn`
Example: `-spath/to/source` or `--Source=path/to/source`

### Parser Option

Shortcut: -p
Long: --Parser
Description: Specifies the parser to use (YAML, JSON, XML).
Accept: single value.
Mandatory: no expect if `--StdIn` is specified. When omitted Didot will select the parser based on the extension of the source file
Example: `-pYAML` or `--Parser=YAML`

### Parser files' extension association option

Shortcut: -X
Long: --ParserExtension
Description: Specifies additional or replacing association between a file extension and a parser for automatic detection
Accept: multiple key-value pairs.
Mandatory: no.
Example: `-xtxt:yaml;dat:json` or `--ParserExtension=txt:yaml;dat:json`

### StdIn Option

Shortcut: `-i`
Long: --StdIn
Description: Specifies the input to the source data as coming from the StdIn.
Accept: switch value.
Exclusive: can't be set with the parameter `--Source`
Example: `-i` or `--StdIn`

### Output Option

Shortcut: `-o`
Long: `--Output`
Description: Specifies the path to the generated output file. If omitted, output is rendered to StdOut.
Accept: single value.
Mandatory: no.
Example: `-opath/to/output` or `--Output=path/to/output`

