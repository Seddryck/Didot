---
title: Specify parser's parameters
tags: [cli-usage]
---
## Direct specification of data parser's parameters

- `-P, --parser-parameter`: Defines the parser's parameters (or configuraion) to use when using the parser. Accepted values have the form `file extension` followed by `@` followed by the `parameter name` and then `:` to end with the `parameter value`. This option is always optional and is only supported for a few specific parsers and with a predefined set of parameters for each of them.

```bash
didot -t template.hbs -s data.tsv -X tsv:csv -P tsv@delimiter:Semicolumn;tsv@commentChar:#
```

In this example:

- `template.hbs` is the Handlebars template file.
- `data.txt` is the source file.
- `tsv:csv` associates the extension `.tsv` to the CSV parser
- `tsv@delimiter:Semicolumn;tsv@commentChar:#` defines two parameters for the configuration of parser associated to `.tsv` file's extension. The first one defines the parameter `delimiter` to a semicolumn (`;`) and the second one set the parameter `commentChar` to a hash (`#`).

## Parameters for CSV parsers

The following parameters are accepted by Didot to define the behavior of a CSV parser:

- `delimiter`: Specifies the delimiter between fields.
- `lineTerminator`: Specifies the delimiter between records.
- `quoteChar`: Character used to quote fields.
- `doubleQuote`: Whether double quotes are used to escape quotes within quoted fields.
- `escapeChar`: Character used for escaping.
- `header`: Indicates if the first row contains headers.
- `headerRows`: Indexes of rows to consider like headers.
- `headerJoin`: concatenor between two headers from different rows.
- `skipInitialSpace`: Whether spaces after delimiters are skipped.
- `commentChar`: Character used to denote comments.
- `commentRows`: Indexes of rows to consider like comments

More information about these parameters can found in [the documentation of PocketCsvReader](https://seddryck.github.io/PocketCsvReader/docs/csv-dialect-descriptor/)

### Synonyms for parameter values

To avoid conflicts with other parts of the command line, Didot supports the following synonyms for parameter values:

#### **delimiter**

- `Comma` = `,`
- `Semicolon` = `;`
- `Tab` = `\t`
- `Pipe` = `|`

#### **lineTerminator**

- `CarriageReturnLineFeed` = `\r\n`
- `LineFeed` = `\n`
- `CarriageReturn` = `\r`

#### **quoteChar**

- `DoubleQuote` = `"`
- `SingleQuote` = `'`

#### **escapeChar**

- `BackSlash` = `\`
- `ForwardSlash` = `/`

#### **commentChar**

- `Hash` = `#`
- `Semicolon` = `;`
- `ForwardSlash` = `/`
- `Dash` = `-`
