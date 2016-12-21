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
        private Boolean finalized = false;

        string TexCodeTemplate = null;
        string TexImageTemplate = null;
        string TexPDFTemplate = null;

        public TexMaker()
        {
            content = "";

            //try
            //{
            //    TexCodeTemplate = System.IO.File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\TexCodeTemplate.tex"));
            //    TexImageTemplate = System.IO.File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\TexImageTemplate.tex"));
            //    TexPDFTemplate = System.IO.File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\TexPDFTemplate.tex"));
            //}
            //catch(Exception ex)
            //{
            //    string outputString = ex.ToString();
            //    System.IO.File.WriteAllText((System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\CrashLog.txt"), outputString);
            //    outputString = null;
            //}
        }

        private TexMaker addPremable()
        {
            string path = Path.Combine(Environment.CurrentDirectory, @"Resources\premable.tex");
            content += File.ReadAllText(path);
            return this;
        }

        public String build()
        {
            if (!finalized)
                finalize();
            return content;
        }

        public TexMaker finalize()
        {
            if (finalized)
                return this;
            //content += "\\end{document}";
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
