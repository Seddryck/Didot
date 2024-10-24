using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Didot.Core.TemplateEngines;
using NUnit.Framework;

namespace Didot.Core.Testing.TemplateEngines;
public class FluidWrapperTests : DotLiquidWrapperTests
{
    protected override ITemplateEngine GetEngine()
        => new FluidWrapper();
}
