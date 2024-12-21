using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core.SourceParsers;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class ExtensionAttribute : Attribute
{
    public string[] Extensions { get; }

    public ExtensionAttribute(params string[] extensions)
    {
        if (extensions == null || extensions.Length == 0)
        {
            throw new ArgumentException("At least one file extension must be provided.", nameof(extensions));
        }

        Extensions = extensions;
    }
}
