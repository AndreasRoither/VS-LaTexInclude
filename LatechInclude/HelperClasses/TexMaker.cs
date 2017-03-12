using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace LaTexInclude.HelperClasses
{
    internal class TexMaker
    {
        private String _content = null;
        private Boolean _missingFiles = false;
        private Boolean finalized = false;
        private string TexCodeTemplate = null;
        private string TexImageTemplate = null;
        private string TexPDFTemplate = null;

        private List<string> _lines = null;

        /// <summary>
        /// TexMaker Constructor
        /// </summary>
        public TexMaker()
        {
            _lines = new List<string>();

            if (!LoadFiles())
            {
                _missingFiles = true;
            }
        }

        public string Content
        {
            get
            {
                return _content;
            }
        }

        public bool MissingFiles
        {
            get
            {
                return _missingFiles;
            }
        }

        private TexMaker addPremable()
        {
            string path = Path.Combine(Environment.CurrentDirectory, @"Resources\premable.tex");
            _content += File.ReadAllText(path);
            return this;
        }

        public String build()
        {
            if (!finalized)
                finalize();
            return _content;
        }

        public TexMaker addImage(String relativePath, float size)
        {
            if (finalized)
                return this;

            //TODO: Convert Path to Latex Format (Replace \ by /)
            _content += "\\begin{figure}[H]\n\\centering\n\\includegraphics[scale = " + size + "]{" + relativePath + "}\n\\end{figure}";
            return this;
        }

        public TexMaker addPDF()
        {
            if (finalized)
                return this;
            return this;
        }

        public TexMaker addCode()
        {
            if (finalized)
                return this;
            return this;
        }

        public TexMaker finalize()
        {
            if (finalized)
                return this;
            //content += "\\end{document}";
            finalized = true;
            return this;
        }

        /// <summary>
        /// Loads or creates the needed files for the texmaker
        /// </summary>
        /// <returns>true if all files found / false if one file is missing</returns>
        private bool LoadFiles()
        {
            bool missingFiles = false;
            string message = "Missing ";
            string path_temp = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (!Directory.Exists(Path.Combine(path_temp, "\\Resources")))
            {
                Directory.CreateDirectory(path_temp + "\\Resources");
            }

            if (File.Exists(Path.Combine(path_temp, @"Resources\TexCodeTemplate.tex")))
            {
                string[] TexCodeLines = System.IO.File.ReadAllLines(Path.Combine(path_temp, @"Resources\TexCodeTemplate.tex"));
            }
            else
            {
                string exampleLine = "\\lstinputlisting[language=$Language$] {$Path$}";

                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter((path_temp + "\\Resources\\TexCodeTemplate.tex")))
                {
                    file.WriteLine(exampleLine);
                }
                message += "CodeTemplate";
                missingFiles = true;
            }

            if (File.Exists(Path.Combine(path_temp, @"Resources\TexImageTemplate.tex")))
            {
                string[] TexImageLines = System.IO.File.ReadAllLines(Path.Combine(path_temp, @"Resources\TexImageTemplate.tex"));
            }
            else
            {
                string exampleLine = "\\begin{figure}[H]\n\\centering\n\\includegraphics[scale = { Scale }]{ { $Path$ } }\n" +
                    "\\caption{ { Caption} }\n\\label{ fig: { Label } }\n\\end{ figure}";

                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter((System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Resources\\TexImageTemplate.tex")))
                {
                    file.WriteLine(exampleLine);
                }
                message += "ImageTemplate ";
                missingFiles = true;
            }

            if (File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\TexPDFTemplate.tex")))
            {
                string[] TexPdfLines = System.IO.File.ReadAllLines(Path.Combine(path_temp, @"Resources\TexPDFTemplate.tex"));
            }
            else
            {
                string exampleLine = "\\includepdf[pages=1-,pagecommand={}]{{ $Path$ }}";

                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter((path_temp + "\\Resources\\TexPDFTemplate.tex")))
                {
                    file.WriteLine(exampleLine);
                }
                message += "PDFTemplate ";
                missingFiles = true;
            }

            if (missingFiles)
            {
                return false;
            }

            return true;
        }
    }
}