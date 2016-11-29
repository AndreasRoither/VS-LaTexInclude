using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatechInclude.HelperClasses
{
    class TexTemplate_Code : TexTemplate
    {
        public TexTemplate_Code(string Type, string Path, string Language) : base(Type, Path)
        {
            this.Type = Type;
            this.Path = Path;
            this.Language = Language;
        }

        public string Language { get; set; }
    }
}
