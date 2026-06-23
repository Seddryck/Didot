---
title: Extension concept
tags: [extensions, cli-usage]
---
## Overview

An extension lets you customize Didot's rendering pipeline by injecting your own code.

At runtime, Didot loads registered extension assemblies and executes hooks on the model **before** template rendering.

## What an extension does

A Didot extension implements `IPipelineExtensionHook`:

- Input: the current model object.
- Output: the transformed model object.

This allows scenarios such as:

- adding computed fields,
- normalizing or reshaping model data,
- enforcing domain-specific transformations before rendering.

## Runtime behavior

During `didot` execution:

1. Didot reads the extension registry file.
2. Enabled extension sources are resolved to assembly paths.
3. Each assembly is loaded.
4. A compatible hook type (`IPipelineExtensionHook` + `DidotExtensionAttribute`) is instantiated.
5. Hooks are executed in registration order.

If no registry file is present, Didot runs normally without extension hooks.
