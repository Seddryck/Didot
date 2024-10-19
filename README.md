# Didot

![Logo](https://raw.githubusercontent.com/Seddryck/Didot/main/assets/didot-logo-256.png)

Transform your structured YAML, JSON or XML data into beautiful, fully-customized HTML pages or plain text in seconds with Didot. This command-line tool allows you to seamlessly generate renders from data files using your preferred templates. Whether you're building static sites, documentation, or reporting tools, Didot makes it easy to turn raw data into polished, web-ready content.

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

### Install as a .NET global tool

A .NET global tool is a console application that you can install and run from any directory on your machine. Here’s a guide on how to perform a global installation of a .NET tool:

#### Prerequisites
Before installing a .NET global tool, you must have the .NET SDK installed on your machine. You can check if it's installed by running the following command in your terminal or Command Prompt:

```bash
dotnet --version
```
If .NET is not installed, download it from [Microsoft's official website](https://dotnet.microsoft.com/download/dotnet).

#### Install a .NET Global Tool
To install a .NET global tool, you use the dotnet tool install command. This command installs a tool for all users globally on your system.

```bash
dotnet tool install -g Didot-cli
```

`-g`: This flag tells the dotnet command to install the tool globally.

#### Verify Installation

After installing the tool, you can verify that it's available globally by running it from any directory.

```bash
didot --version
```

This command will display the installed tool’s version if the installation was successful.

#### Update a .NET Global Tool

To update a globally installed .NET tool, use the dotnet tool update command:

```bash
dotnet tool update -g Didot-cli
```

### Install from GitHub Releases

#### Step 1: Download the ZIP from the GitHub Release

1. Navigate to the **GitHub repository** of the project.
2. Go to the **Releases** section, usually found under the "Code" tab.
3. Download the `.zip` file containing the executable from the desired release.

Example:

   ```
   https://github.com/Seddryck/Didot/releases/latest/
   ```

#### Step 2: Extract the ZIP File

1. Right-click the downloaded `.zip` file and choose **Extract All**.
2. Extract the contents to a directory of your choice, such as `C:\Program Files\Didot`.

> **Tip**: Choose a path that is easy to remember and doesn't contain special characters.

#### Step 3: Add the Executable to the System PATH

To run the executable from any location in the command line, you need to add its folder to your system's PATH.

1. Open the **Start Menu** and search for **Environment Variables**.
2. Click **Edit the system environment variables**.
3. In the **System Properties** window, click **Environment Variables**.
4. In the **System Variables** section, scroll down, select **Path**, and click **Edit**.
5. In the **Edit Environment Variable** dialog, click **New** and enter the path to your extracted folder, e.g., `C:\Program Files\Didot`.
6. Click **OK** to close all windows.

### Step 4: Verify Installation

1. Open **Command Prompt** (CMD).
2. Type the name of the executable (e.g., `didot.exe`) and hit Enter.
3. If everything is set up correctly, the program should run.

## QuickStart

**Didot** is a command-line tool designed for generating files based on templating. It supports *YAML*, *JSON*, and *XML* as source data formats and provides flexibility in templating through both *Scriban*, *Liquid*, *Handlebars* and *SmartFormat* templates languages. With Didot, you can easily automate file generation by combining structured data from YAML, JSON, or XML files with customizable templates using Scriban or DotLiquid.

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
  - Typical Use Case: Config file generation, reports, email templates, or any templating scenario not tied to a specific web framework.
- **Liquid**: Templates with the `.liquid` extension are parsed using a dotLiquid template engine. DotLiquid is a .NET port of the Liquid templating engine used by platforms like Shopify.
  - Secure (no access to system objects), making it ideal for user-generated templates.
  - Allows both dynamic and static templating.
  - Supports filters, tags, and various control flow structures.
  - Typical Use Case: SaaS applications, dynamic content rendering, email templates.
- **Handlebars**: Templates with the `.hbs` extension are parsed using a Handlebars template engine. Handlebars C# port of the popular JavaScript Handlebars templating engine.
  - Simple syntax for generating HTML or text files from templates.
  - Support for helpers, partial templates, and block helpers.
  - Good separation of logic from presentation.
  - Typical Use Case: Email templates, reports, and content generation.
- **SmartFormat**: Templates with the `.smart` extension are parsed using a SmartFormat template engine. SmartFormat.Net is a A lightweight templating engine primarily used for string formatting.
  - Provides more advanced formatting capabilities than standard string formatting in C#.
  - Supports nested templates, conditional formatting, and more.
  - Typical Use Case: Log messages, report generation, and dynamic text formatting.

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

* `template.scriban` is the Scriban template file.
* `data.yaml` is the source file containing the structured data in YAML format.
* `page.html` is the output file that will contain the generated content.

Make sure that the template file and source file are correctly formatted and aligned with your data model to produce the desired result.
