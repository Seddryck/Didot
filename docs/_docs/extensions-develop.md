---
title: Develop an extension
tags: [extensions, development]
---
## 1. Create a .NET class library

Create a class library targeting a compatible .NET runtime and reference `Didot.Core`.

## 2. Add extension metadata

Add assembly-level metadata (required for registration metadata discovery):

```csharp
using Didot.Core;

[assembly: DidotExtension("my.company.transform", "My Transform Extension")]
```

## 3. Implement a pipeline hook

Create a class that:

- implements `IPipelineExtensionHook`,
- is decorated with `DidotExtensionAttribute`,
- has a public parameterless constructor.

```csharp
using Didot.Core;

[DidotExtension("my.company.transform", "My Transform Hook")]
public sealed class MyTransformHook : IPipelineExtensionHook
{
    public object Apply(object model)
    {
        // transform and return model
        return model;
    }
}
```

## 4. Build and register

Build the project and register the produced DLL:

```bash
didot extensions register ./bin/Debug/net8.0/MyExtension.dll
```

Then run Didot as usual (`didot -t ... -s ...`).

## 5. Design guidance

- Keep `Apply` deterministic and side-effect free when possible.
- Always return a non-null model.
- Throw meaningful exceptions for invalid model states.
- Keep hook logic focused on model transformation (not template rendering).
