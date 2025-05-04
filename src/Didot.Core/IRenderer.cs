using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotLiquid;

namespace Didot.Core;
public interface IRenderer
{
    string Render(object model);
}
