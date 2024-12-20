using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PocketCsvReader;
using System.IO;

namespace Didot.Core.SourceParsers;
public class CsvSource : ISourceParser
{
    private CsvReader CsvReader { get; }

    public CsvDialectDescriptor Dialect { get => CsvReader.Dialect; }

    public CsvSource()
        => CsvReader = new CsvReader(new CsvProfile(true));

    public CsvSource(CsvReader csvReader)
        => CsvReader = csvReader;

    public virtual object Parse(string content)
    {
        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        return Parse(memoryStream);
    }

    public virtual object Parse(Stream stream)
    {
        var list = new List<object>();

        using var reader = CsvReader.ToDataReader(stream);
        var seenRecords = new Dictionary<string, Dictionary<string, object>>();

        while (reader.Read())
        {
            var rowTopLevelKey = new List<string>();
            var rowDictionary = new Dictionary<string, object>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string header = reader.GetName(i);
                string value = reader.GetString(i);

                // Build nested properties in the dictionary
                AddNestedProperty(rowDictionary, header, value);

                // Keep track of top-level keys for deduplication
                if (!header.Contains("[]") && !header.Contains("."))
                {
                    rowTopLevelKey.Add(value);
                }
            }

            // Generate a unique key for deduplication based on top-level properties
            string uniqueKey = string.Join("|", rowTopLevelKey);
            if (!seenRecords.ContainsKey(uniqueKey))
            {
                seenRecords[uniqueKey] = rowDictionary;
                list.Add(rowDictionary);
            }
            else
            {
                // Merge "list" fields if the record already exists
                MergeLists(seenRecords[uniqueKey], rowDictionary);
            }
        }

        // Return the list or a single object if only one exists
        return list.Count != 1 ? list : list[0];
    }

    private static void AddNestedProperty(Dictionary<string, object> dictionary, string header, string value)
    {
        var parts = header.Split('.');
        var currentDict = dictionary;

        for (int i = 0; i < parts.Length; i++)
        {
            string part = parts[i];
            bool isList = part.EndsWith("[]");

            // Remove the [] notation for key manipulation
            string key = isList ? part.TrimEnd('[', ']') : part;

            if (i == parts.Length - 1)
            {
                // Last part of the header, assign the value
                if (isList)
                {
                    if (!currentDict.ContainsKey(key))
                    {
                        currentDict[key] = new List<Dictionary<string, object>>();
                    }

                    // Add the value as a dictionary
                    var list = (List<Dictionary<string, object>>)currentDict[key];
                    list.Add(new Dictionary<string, object> { { key, value } });
                }
                else
                {
                    currentDict[key] = value;
                }
            }
            else
            {
                // Intermediate part, create or reuse nested dictionary or list
                if (!currentDict.ContainsKey(key))
                {
                    currentDict[key] = isList ? (object)new List<Dictionary<string, object>>() : new Dictionary<string, object>();
                }

                if (isList)
                {
                    var list = (List<Dictionary<string, object>>)currentDict[key];
                    if (list.Count == 0)
                    {
                        list.Add(new Dictionary<string, object>());
                    }

                    // Move to the first dictionary in the list
                    currentDict = list[0];
                }
                else
                {
                    currentDict = (Dictionary<string, object>)currentDict[key];
                }
            }
        }
    }

    private static void MergeLists(Dictionary<string, object> existing, Dictionary<string, object> newRecord)
    {
        foreach (var key in newRecord.Keys)
        {
            if (existing.ContainsKey(key) && existing[key] is List<Dictionary<string, object>> existingList)
            {
                // Merge lists
                var newList = (List<Dictionary<string, object>>)newRecord[key];
                existingList.AddRange(newList);
            }
            else if (!existing.ContainsKey(key))
            {
                existing[key] = newRecord[key];
            }
        }
    }
}
