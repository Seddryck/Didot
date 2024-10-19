using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Didot.Core.SourceParsers;
public class XmlSource : ISourceParser
{
    public object Parse(string content)
    {
        var doc = new XmlDocument();
        doc.LoadXml(content);
        return ParseXmlDocument(doc);
    }

    public object Parse(Stream stream)
    {
        var doc = new XmlDocument();
        doc.Load(stream);
        return ParseXmlDocument(doc);
    }

    private static object ParseXmlDocument(XmlDocument doc)
    {
        if (AllChildNodesAreSame(doc.DocumentElement!) && doc.DocumentElement!.ChildNodes.Count > 1)
        {
            var list = new List<object>();
            foreach (XmlNode child in doc.DocumentElement!.ChildNodes)
                list.Add(ParseXmlNode(child));
            return list;
        }
        else
            return ParseXmlNode(doc.DocumentElement!);
    }

    private static object ParseXmlNode(XmlNode node)
    {
        if (node.HasChildNodes)
        {
            if (AllChildNodesAreSame(node) && node.ChildNodes.Count > 1)
            {
                var list = new List<object>();
                foreach (XmlNode child in node.ChildNodes)
                {
                    list.Add(ParseXmlNode(child));
                }
                return list;
            }
            else
            {
                var dict = new Dictionary<string, object>();
                foreach (XmlNode child in node.ChildNodes)
                {
                    if (child.NodeType == XmlNodeType.Element)
                    {
                        if (child.ChildNodes.Count ==1 && child.ChildNodes[0]!.NodeType == XmlNodeType.Text)
                            dict[child.Name] = child.ChildNodes[0]!.Value!;
                        else
                            dict[child.Name] = ParseXmlNode(child);
                    }
                }
                return dict;
            }
        }
        else
            return node.InnerText;
    }

    private static bool AllChildNodesAreSame(XmlNode node)
    {
        if (node.ChildNodes.Count == 0) return false;

        string firstChildName = node.ChildNodes[0]!.Name;
        foreach (XmlNode child in node.ChildNodes)
            if (child.Name != firstChildName) return false;
        return true;
    }
}
