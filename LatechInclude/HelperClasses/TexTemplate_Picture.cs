using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatechInclude.HelperClasses
{
    class TexTemplate_Picture : TexTemplate
    {
        public TexTemplate_Picture(string Type, string Path, float Scale, string Label, string Caption) : base(Type, Path)
        {
            this.Type = Type;
            this.Path = Path;
            this.Scale = Scale;
            this.Label = Label;
            this.Caption = Caption;
        }

        public float Scale { get; set; }

        public string Label { get; set; }

        public string Caption { get; set; }
    }
}
