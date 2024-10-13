using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Didot.Core;
public interface ISourceParser
{
    object Parse(string content);
    object Parse(StreamReader content);
}
