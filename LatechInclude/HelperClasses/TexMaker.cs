﻿using System;
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
        private Boolean finalized = false;

        public TexMaker(){
            content = "";
            addPremable();
            content += "\\begin{document}\n";
        }

        private TexMaker addPremable()
        {
            string path = Path.Combine(Environment.CurrentDirectory, @"Resources\premable.tex");
            content += File.ReadAllText(path);
            return this;
        }

        public String build(){
            if (!finalized)
                finalize();
            return content;
        }

        public TexMaker finalize() {
            if (finalized)
                return this;
            content += "\\end{document}";
            finalized = true;
            return this;
        }

        public TexMaker addImage(String relativePath, float size)
        {
            if (finalized)
                return this;

            //TODO: Convert Path to Latex Format (Replace \ by /)
            content += "\\begin{figure}[H]\n\\centering\n\\includegraphics[scale = " + size + "]{" + relativePath + "}\n\\end{figure}";
            return this;
        }

        public TexMaker addCode()
        {
            if (finalized)
                return this;
            return this;
        }

        public TexMaker addPDF()
        {
            if (finalized)
                return this;
            return this;
        }



    }
}