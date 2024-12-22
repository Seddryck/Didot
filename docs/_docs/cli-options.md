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
**Example:** `--template=path/to/template`

## Option Values

### Single Value

An option can take a single value, which is provided right after the shortcut or the `=` sign for long options.
**Example**: `-s path/to/source` or `--source=path/to/source`

### Switch Values

If an option accepts a boolean, this one can be omitted.
**Example**: `-i` or `--stdin` or `--stdin=true` or `--stdin true`

### Multiple Values

If an option accepts multiple values, the values should be separated by a semicolon (;).
**Example**:

- `--sources=file1.yaml;file2.json file3.xml`
- `--sources=file1.yaml file2.json file3.xml`
- `--sources=file1.yaml --sources=file2.json --sources=file3.xml`

### Key-Value Pairs

For key-value pairs, the key is followed by a colon (:), and then the value. Multiple key-value pairs, if allowed, should be separated by a semicolon (;).
**Example**:

- `--parser-extension=txt:handlebars;liquid:fluid`
- `--parser-extension=txt:handlebars liquid:fluid`
- `--parser-extension=txt:handlebars --parser-extension=liquid:fluid`

## Didot Options Explained

### Template option

Shortcut: `-t`
Long: `--template`
Description: Specifies the path to the template file.
Accept: single value.
Mandatory: yes.
Example: `-t path/to/template` or `--template=path/to/template`

### Engine option

- Shortcut: `-e`
- Long: `--engine`
- Description: Specifies the template engine to use (scriban, fluid, dotliquid, handlebars, smartformat, stringtemplate).
- Accept: single value. When omitted Didot will select the engine based on the extension of the template file.
- Example: `-e fluid` or `--engine=fluid`

### Engine files' extension association option

- Shortcut: `-x`
- Long: `--engine-extension`
- Description: Specifies additional or replacing association between a file extension and an engine for automatic detection
- Accept: multiple key-value pairs.
- Mandatory: no.
- Example: `-x txt:handlebars;liquid:fluid` or `--engine-extension=.txt:handlebars;liquid:fluid`

### Source option

- Shortcut: `-s`
- Long: `--source`
- Accept: single value or multiple key-value pairs.
- Description:
  - if single value is provided, it specifies the path to the source file. If omitted, input can be taken from StdIn.
  - if multiple key-value pairs are provided, each of them specifies a part of the model and the key representing the tag in the model.
- Exclusive: can't be set with the parameter `--StdIn`
- Example: `-s path/to/source` or `--source=path/to/source` or `--source=foo:path/to/source1;bar:path/to/source1`

### Parser option

- Shortcut: `-r`
- Long: `--parser`
- Description: Specifies the parser to use (YAML, JSON, XML).
- Accept: single value.
- Mandatory: no expect if `--stdin` is specified. When omitted Didot will select the parser based on the extension of the source file
- Example: `-r YAML` or `--parser=YAML`

### Parser files' extension association option

- Shortcut: `-X`
- Long: `--parser-extension`
- Description: Specifies additional or replacing association between a file extension and a parser for automatic detection
- Accept: multiple key-value pairs.
- Mandatory: no.
- Example: `-X txt:yaml;dat:json` or `--parser-extension=txt:yaml;dat:json`

### Parser's parameters option

- Shortcut: `-P`
- Long: `--parser-parameter`
- Description: Specifies parameters tuning the behaviour of the parser
- Accept: multiple key-value pairs, prefixed by the file's extension followed by an arobas (`@`).
- Mandatory: no.
- Example: `-P csv@delimiter=^;csv@commentChar=#`

### StdIn option

- Shortcut: `-i`
- Long: `--stdin`
- Description: Specifies the input to the source data as coming from the StdIn.
- Accept: switch value.
- Exclusive: can't be set to true with the parameter `--source` and must specified to false when `--source` is not provided.
- Example: `-i` or `--stdin` or `--stdin false`

### Output option

- Shortcut: `-o`
- Long: `--output`
- Description: Specifies the path to the generated output file. If omitted, output is rendered to StdOut.
- Accept: single value.
- Mandatory: no.
- Example: `-o path/to/output` or `--output=path/to/output`
