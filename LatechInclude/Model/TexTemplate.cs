using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatechInclude.HelperClasses
{
    class TexTemplate
    {

        public TexTemplate(string Type, string Path)
        {
            this.Type = Type;
            this.Path = Path;
        }

        public string Path { get; set; }

        public string Type { get; set; }
    }

}
