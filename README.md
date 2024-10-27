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

**Releases:** [![nuget](https://img.shields.io/nuget/v/Didot-cli.svg)](https://www.nuget.org/packages/Didot-cli/) ![Docker Image Version](https://img.shields.io/docker/v/seddryck/didot?label=docker%20hub&color=0db7ed) [![GitHub Release Date](https://img.shields.io/github/release-date/seddryck/Didot.svg)](https://github.com/Seddryck/Didot/releases/latest) [![licence badge](https://img.shields.io/badge/License-Apache%202.0-yellow.svg)](https://github.com/Seddryck/Didot/blob/master/LICENSE) 

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

### Install from Docker

#### Prerequisites

**Docker Installed**: Ensure that Docker is installed and running on your system. You can download Docker from Docker's official site.

#### Pulling the Docker Image

A pre-built Docker image is available on Docker Hub, you can pull it using the following command:

```powershell
docker pull seddryck/didot:latest
```

#### Running Didot from Docker

Once you have the Docker image, you can run Didot using Docker in PowerShell.

###### Basic Command

<sub>CMD:</sub>
```CMD
docker run --rm -v %cd%:/files didot -t <template-file> -s <source-file> -o <output-file>
```

<sub>PowerShell:</sub>
```powershell
docker run --rm -v ${pwd}:/files didot -t <template-file> -s <source-file> -o <output-file>
```

- `--rm`: Automatically removes the container after it finishes executing.
- `-v ${pwd}:/files`: Mounts the current directory (`${pwd}` in PowerShell or Bash, `%cd%` in CMD) to /files inside the Docker container, so Didot can access your local files.
- `-t <template-file>`: Specifies the path to the template file inside the /files directory.
- `-s <source-file>`: Specifies the path to the source file (YAML, JSON, or XML).
- `-o <output-file>`: Specifies the path to the output file that Didot will generate. If omitted, it will display the result on the host console.

##### Example Workflow:

1. Prepare the Template and Source Files:

  - Make sure your template and source files are correctly formatted and saved in the correct directory. For example:
    - `./templates/template-01.hbs`
    - `./data/data.json`
2. Run Didot: Use the following command to generate templated output:

```powershell
docker run --rm -v ${pwd}:/files didot -t /files/templates/template-01.hbs -s /files/data/data.json -o /files/output/output.txt
```

3. Access the Output: The output file will be generated in ./output/output.txt on your local machine after the Docker container finishes execution.

#### Updating Didot

To update to the latest version of Didot, either pull the new Docker image

```powershell
docker pull seddryck/didot:latest
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

**Didot** is a command-line tool designed for generating files based on templating. It supports *YAML*, *JSON*, and *XML* as source data formats and provides flexibility in templating through both *Scriban*, *Liquid*, *Handlebars*, *StringTemplate* and *SmartFormat* templates languages. With Didot, you can easily automate file generation by combining structured data from YAML, JSON, or XML files with customizable templates using Scriban or Liquid.

### Supported Data Formats:

- **YAML**: Files with the `.yaml` or `.yml` extension are parsed using a YAML source parser.
- **JSON**: Files with the `.json` extension are parsed using a JSON source parser.
- **XML**: Files with the `.xml` extension are parsed using an XML source parser.
- **FrontMatterMarkdown**: Files with the `.md` extension are parsed using an YAML parser for the FrontMatter and the Markdown content is added in the entry *content*.
- **FrontMatter**: using an YAML parser for the FrontMatter, the Markdown content is not appended to the result.

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
- **StringTemplate**: Templates with the `.st` and `.stg` extension are parsed using the StringTemplate engine. StringTemplate is a powerful template engine specifically designed to enforce strict separation of logic from presentation.
  - Focused on generating structured text, such as code, XML, and reports.
  - Strong emphasis on enforcing Model-View separation.
  - Supports conditionals, loops, and automatic escaping to prevent security issues.
  - Typical Use Case: Code generation, configuration files, and situations where strict separation between logic and template is required.

### Command Usage:

The command to run Didot is simply `didot`. When executing it, you need to provide three required arguments:

- `-t, --template` (required): Specifies the path to the Scriban, Liquid, Handlebars, StringTemplate or SmartFormat template file.
- `-s, --source`: Specifies the path to the source data file, which can be in YAML, JSON, or XML format. If this argument is not provided, the data will be read from the console input. In such cases, the `-r, --parser` option becomes mandatory.
- `-o, --output`: Specifies the path to the output file where the generated content will be saved. If not provided, the output will be displayed directly in the console.

**Example:**

```bash
didot -t template.scriban -s data.yaml -o page.html
```

In this example:

- `template.scriban` is the Scriban template file.
- `data.yaml` is the source file containing the structured data in YAML format.
- `page.html` is the output file that will contain the generated content.

### List of options

### Template option

- Shortcut: `-t`
- Long: `--template`
- Description: Specifies the path to the template file.
- Accept: single value.
- Mandatory: yes.
- Example: `-t path/to/template` or `--template=path/to/template`

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

#### Example:

##### With a source file:

```bash
didot -t template.scriban -s data.yaml -o page.html
```

In this example:

- `template.scriban` is the Scriban template file.
- `data.yaml` is the source file containing the structured data in YAML format.
- `page.html` is the output file that will contain the generated content.

##### With data from the console:

<sub>CMD:</sub>
```cmd
type "data.json" | didot --stdin -t template.hbs -r json
```

<sub>PowerShell:</sub>
```powershell
Get-Content data.json | didot --stdin -t template.hbs -r json
```

<sub>Bash:</sub>
```bash
cat data.json | didot --stdin -t template.hbs -r json
```

In this example:

- The input data is coming from the console
- - `template.hbs` is the Handlebars template file.
- `json` is the parser of input data.
- the output is redirected to the console.

Make sure that the template file and source file are correctly formatted and aligned with your data model to produce the desired result.
