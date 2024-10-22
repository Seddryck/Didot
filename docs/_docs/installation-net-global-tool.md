---
title: Install as a .NET global tool
tags: [installation]
---
A .NET global tool is a console application that you can install and run from any directory on your machine. Here's a guide on how to perform a global installation of a .NET tool:

## Prerequisites

Before installing a .NET global tool, you must have the .NET SDK installed on your machine. You can check if it's installed by running the following command in your terminal or Command Prompt:

```bash
dotnet --version
```

If .NET is not installed, download it from [Microsoft's official website](https://dotnet.microsoft.com/download/dotnet).

## Install a .NET Global Tool

To install a .NET global tool, you use the dotnet tool install command. This command installs a tool for all users globally on your system.

```bash
dotnet tool install -g Didot-cli
```

`-g`: This flag tells the dotnet command to install the tool globally.

## Verify Installation

After installing the tool, you can verify that it's available globally by running it from any directory.

```bash
didot --version
```

This command will display the installed tool's version if the installation was successful.

## Update a .NET Global Tool

To update a globally installed .NET tool, use the dotnet tool update command:

```bash
dotnet tool update -g Didot-cli
```
