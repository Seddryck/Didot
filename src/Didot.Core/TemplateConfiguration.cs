using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Didot.Core;
public record TemplateConfiguration
(
    bool HtmlEncode = false
)
{ }
