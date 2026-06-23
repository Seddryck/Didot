---
title: Register an extension
tags: [extensions, cli-usage]
---
## Command

Use the extension registration command:

```bash
didot extensions register <reference>
```

Optional friendly name override:

```bash
didot extensions register <reference> --name "My Extension"
```

## Supported references

`<reference>` can be:

- a DLL path (absolute or relative),
- a directory containing exactly one DLL,
- a non-path identifier (extension id, extension name, or assembly name).

Examples:

```bash
didot extensions register ./Didot.Expressif.dll
didot extensions register ./extensions/Didot.Expressif/
didot extensions register Didot.Expressif
```

## Lookup order for non-path references

When `<reference>` is not a path, Didot searches in this order:

1. current directory
2. current directory `extensions/`
3. Didot installation directory `extensions/`
4. user directory `~/.didot/extensions/`

If multiple matches are found, registration fails and you must provide an explicit path.

## Registry behavior

On success, Didot stores the extension in `didot.extensions.registry.json` with:

- id,
- name,
- assembly path,
- enabled flag,
- version,
- registration timestamp.

Didot rejects duplicate registrations by id or assembly path.
