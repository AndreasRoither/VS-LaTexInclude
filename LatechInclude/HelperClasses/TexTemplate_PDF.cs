using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatechInclude.HelperClasses
{
    class TexTemplate_PDF : TexTemplate
    {
        public TexTemplate_PDF(string Type, string Path) : base(Type, Path)
        {
            this.Type = Type;
            this.Path = Path;
        }
    }
}
