using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LatechInclude.HelperClasses
{
    class TexMaker
    {

        private String content = null;

        public TexMaker(){
            content = "";
            addPremable();
            content += "\\begin{document}\n";
        }

        public String getContent(){
            return content;
        }

        public void finalize() {
            content += "\n\\end{document}";
        }

        private void addPremable(){
            string path = Path.Combine(Environment.CurrentDirectory, @"Resources\premable.tex");
            content += File.ReadAllText(path);
        }

        public void addImage()
        {

        }

        public void addCode()
        {

        }

        public void addPDF()
        {

        }



    }
}
