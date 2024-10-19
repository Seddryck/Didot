# Didot

![Logo](https://raw.githubusercontent.com/Seddryck/Didot/main/assets/didot-logo-256.png)

Transform your structured YAML data into beautiful, fully-customized HTML pages in seconds with Didot. This command-line tool allows you to seamlessly generate HTML from YAML files using your preferred templates. Whether you're building static sites, documentation, or reporting tools, Didot makes it easy to turn raw data into polished, web-ready content.

[About][] | [Installing][] | [Quickstart][]

[About]: #about (About)
[Installing]: #installing (Installing)
[Quickstart]: #quickstart (Quickstart)

## About

**Social media:** [![website](https://img.shields.io/badge/website-seddryck.github.io/Didot-fe762d.svg)](https://seddryck.github.io/Didot)
[![twitter badge](https://img.shields.io/badge/twitter%20Didot-@Seddryck-blue.svg?style=flat&logo=twitter)](https://twitter.com/Seddryck)

**Releases:** [![nuget](https://img.shields.io/nuget/v/Didot-cli.svg)](https://www.nuget.org/packages/Didot-cli/) <!-- [![GitHub Release Date](https://img.shields.io/github/release-date/seddryck/Didot.svg)](https://github.com/Seddryck/Didot/releases/latest) --> [![licence badge](https://img.shields.io/badge/License-Apache%202.0-yellow.svg)](https://github.com/Seddryck/Didot/blob/master/LICENSE)

**Dev. activity:** [![GitHub last commit](https://img.shields.io/github/last-commit/Seddryck/Didot.svg)](https://github.com/Seddryck/Didot/commits)
![Still maintained](https://img.shields.io/maintenance/yes/2024.svg)
![GitHub commit activity](https://img.shields.io/github/commit-activity/y/Seddryck/Didot)

**Continuous integration builds:** [![Build status](https://ci.appveyor.com/api/projects/status/na3dklqjsuv1lbfv?svg=true)](https://ci.appveyor.com/project/Seddryck/Didot/)
[![Tests](https://img.shields.io/appveyor/tests/seddryck/Didot.svg)](https://ci.appveyor.com/project/Seddryck/Didot/build/tests)
[![CodeFactor](https://www.codefactor.io/repository/github/seddryck/Didot/badge)](https://www.codefactor.io/repository/github/seddryck/Didot)
[![codecov](https://codecov.io/github/Seddryck/Didot/branch/main/graph/badge.svg?token=YRA8IRIJYV)](https://codecov.io/github/Seddryck/Didot)
<!-- [![FOSSA Status](https://app.fossa.com/api/projects/git%2Bgithub.com%2FSeddryck%2FDidot.svg?type=shield)](https://app.fossa.com/projects/git%2Bgithub.com%2FSeddryck%2FDidot?ref=badge_shield) -->

**Status:** [![stars badge](https://img.shields.io/github/stars/Seddryck/Didot.svg)](https://github.com/Seddryck/Didot/stargazers)
[![Bugs badge](https://img.shields.io/github/issues/Seddryck/Didot/bug.svg?color=red&label=Bugs)](https://github.com/Seddryck/Didot/issues?utf8=%E2%9C%93&q=is:issue+is:open+label:bug+)
[![Top language](https://img.shields.io/github/languages/top/seddryck/Didot.svg)](https://github.com/Seddryck/Didot/search?l=C%23)

## Installing

```bash
dotnet tool install -g Didot-cli
```

## QuickStart

**Didot** is a command-line tool designed for generating files based on templating. It supports both YAML and JSON as source data formats and uses Scriban as its templating engine. With Didot, you can easily automate file generation by combining structured data from YAML or JSON files with customizable templates.

### Supported Data Formats:

- **YAML**: Files with the `.yaml` or `.yml` extension are parsed using a YAML source parser.
- **JSON**: Files with the `.json` extension are parsed using a JSON source parser.
- **XML**: Files with the `.xml` extension are parsed using an XML source parser.

### Supported Templating Engines:

Didot utilizes some templating engines, which allow for powerful and flexible templating.

- **Scriban**: Templates with the `.scriban` extension are parsed using a Scriban template engine. Scriban is a lightweight and fast template engine with rich support for multiple output formats.
  - Highly performant, designed to handle large-scale template processing.
  - Supports customizable scripting with rich expressions and filters.
  - Can work with JSON and YAML data sources.
- **dotLiquid**: Templates with the `.liquid` extension are parsed using a dotLiquid template engine. DotLiquid is a .NET port of the Liquid templating engine used by platforms like Shopify.
  - Secure (no access to system objects), making it ideal for user-generated templates.
  - Allows both dynamic and static templating.
  - Supports filters, tags, and various control flow structures.

### Command Usage:

The command to run Didot is simply `didot`. When executing it, you need to provide three required arguments:

- `-t, --Template`: Specifies the path to the Scriban template file.
- `-s, --Source`: Specifies the path to the source data file, which can be in YAML or JSON format.
- `-o, --Output`: Specifies the path to the output file where the generated content will be saved.

#### Example:

```bash
didot -t template.scriban -s data.yaml -o page.html
```

In this example:

* template.scriban is the Scriban template file.
* data.yaml is the source file containing the structured data in YAML format.
* result.txt is the output file that will contain the generated content.

Make sure that the template file and source file are correctly formatted and aligned with your data model to produce the desired result.
