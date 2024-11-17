---
title: CSV parser
tags: [parsers]
---
Didot leverages the [PocketCsvReader](http://github.com/Seddryck/PocketCsvReader) parser and interprets CSV headers containing `.` and `[]`, dynamically transforming them into structured nested dictionaries or lists, enabling seamless representation of hierarchical data.

## Default CSV Dialect

The parser assumes the following characteristics for the CSV file:

* Delimiter: The default column delimiter is a comma (,).
* Quotation: Fields containing delimiters, line breaks, or special characters are enclosed in double quotes (").
* Escape Character: Double quotes within a quoted field are escaped by doubling them ("").
* Header Row: The first row of the CSV file must contain column headers, which define the structure of the resulting nested dictionary.
* Case Sensitivity: Headers are case-sensitive.
* Row Order: Rows with the same parent-level fields are grouped together in the final output. The order of rows in the CSV affects the resulting data structure.

```text
Name.Fullname,Name.Acronym,Founded
NASA,National Aeronautics and Space Administration,1958-07-29
```

## Special Header Syntax in Didot

### Dot Notation (.)

A dot (`.`) in a header is interpreted by the parser as indicating a nested property structure.

Each segment before a dot represents a key in a nested dictionary.
The final segment represents the value assigned within that dictionary.

#### Example of nested properties

| *Header*       | *Interpretation*                                         |
|-------------------|-----------------------------------------------------------|
| `Name.Fullname`   | Creates a dictionary `Name` with a key `Fullname`.         |
| `Name.Acronym`    | Adds a key `Acronym` to the `Name` dictionary.             |
| `Founded`         | Adds a top-level key `Founded`.                           |

is the equivalent of this JSON representation

```json
{
  "Name": {
    "Fullname": "NASA",
    "Acronym": "National Aeronautics and Space Administration"
  },
  "Founded": "1958-07-29"
}
```

### List Syntax ([])

Square brackets (`[]`) in a header indicate that the corresponding property belongs to a list of dictionaries.

* Headers containing `[]` represent list elements.
* Each row with the same parent-level fields contributes to the same list.
* Nested properties of list items are denoted by combining `[]` with dot notation.

#### Example of list

| *Header*       | *Interpretation*                                         |
|-------------------|-----------------------------------------------------------|
| `Buildings[].Name`   | Creates a list `Buildings` where each item has a key `Name`.         |
| `Buildings[].Type`    | Adds a key `Type` to each `Buildings` list item.             |
| `Buildings[].Location` | Adds a key `Location` to each `Buildings` list item.  |

is the equivalent of this JSON representation

```json
{
  "Buildings": [
    {
      "Name": "NASA Headquarters",
      "Type": "Administration and Policy",
      "Location": "300 E Street SW, Washington, DC 20546"
    },
    {
      "Name": "Johnson Space Center",
      "Type": "Human Spaceflight Training, Research, and Mission Control",
      "Location": "2101 NASA Parkway, Houston, TX 77058"
    }
  ]
}
```

### Combining . and []

Dot notation and square brackets can be combined in Didot to describe deeply nested structures, such as lists within nested dictionaries.

#### Example of combination

| *Header*       | *Interpretation*                                         |
|-------------------|-----------------------------------------------------------|
| `Buildings[].Details.Address.City`   | Adds a nested dictionary `Details` to each `Buildings` list item.  |
| `Buildings[].Details.Address.ZipCode`    | Adds a key `ZipCode` to Address in each `Buildings` list item. |

is the equivalent of this JSON representation

```json
{
  "Buildings": [
    {
      "Details": {
        "Address": {
          "City": "Washington",
          "ZipCode": "20546"
        }
      }
    },
    {
      "Details": {
        "Address": {
          "City": "Houston",
          "ZipCode": "77058"
        }
      }
    }
  ]
}
```

### Behavior of Rows with Repeated Parent-Level Fields

In Didot, rows with the same values for all non-list fields are considered part of the same top-level object. List fields (`[]`) for such rows are appended to the corresponding lists.

```text
Name.Fullname,Name.Acronym,Founded,Buildings[].Name,Buildings[].Type
NASA,National Aeronautics and Space Administration,1958-07-29,NASA Headquarters,Administration and Policy
NASA,National Aeronautics and Space Administration,1958-07-29,Johnson Space Center,Human Spaceflight Training, Research, and Mission Control
```

#### Resulting Structure

```json
[
  {
    "Name": {
      "Fullname": "NASA",
      "Acronym": "National Aeronautics and Space Administration"
    },
    "Founded": "1958-07-29",
    "Buildings": [
      {
        "Name": "NASA Headquarters",
        "Type": "Administration and Policy"
      },
      {
        "Name": "Johnson Space Center",
        "Type": "Human Spaceflight Training, Research, and Mission Control"
      }
    ]
  }
]
```
