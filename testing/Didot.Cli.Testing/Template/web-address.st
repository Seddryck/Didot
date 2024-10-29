# <model.Hostname>

## Address

- Scheme: <model.Scheme>
- Host: <model.Hostname>
- Port: <if (model.Port)><model.Port><else>No port found.<endif>

## Paths

<if (model.Paths)>
| Index | Path                |
|-------|---------------------|
<model.Paths: { path|
| <i> | /<path> |
}>
<else>
No paths found.
<endif>

## Query Strings

<if (model.QueryStrings)>
| Parameter      | Value         |
|----------------|---------------|
<model.QueryStrings.keys: { key|
| <key> | <model.QueryStrings.(key)> |
}>
<else>
No query strings found.
<endif>

## Section

<if (model.Section)>
<model.Section>
<else>
No section found.
<endif>
