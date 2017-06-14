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
        private string[] TexCodeTemplate = null;
        private string[] TexImageTemplate = null;
        private string[] TexPDFTemplate = null;

        private List<string> _finLines = null;
        TrulyObservableCollection<MyFile> _fileList;
        List<WhiteList> _whiteList;

        /// <summary>
        /// TexMaker Constructor
        /// </summary>
        public TexMaker(TrulyObservableCollection<MyFile> fileList, List<WhiteList> whiteList)
        {
            this._fileList = fileList;
            this._whiteList = whiteList;

            if (!LoadFiles())
            {
                _missingFiles = true;
            }
        }

        /// <summary>
        /// Gets the content
        /// </summary>
        public string Content
        {
            get
            {
                return _content;
            }
        }

        /// <summary>
        /// Gets bool missingFiles
        /// </summary>
        public bool MissingFiles
        {
            get
            {
                return _missingFiles;
            }
        }

        private List<string> AddPremable()
        {
            string path_temp = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (File.Exists(Path.Combine(path_temp, @"Resources\Premable.tex")) | File.Exists(Path.Combine(path_temp, @"Resources\Premable.txt")))
            {
                List<string> temp = new List<string>();
                string line = "";
                System.IO.StreamReader file = new System.IO.StreamReader(Path.Combine(path_temp, @"Resources\Premable.tex"));
                while ((line = file.ReadLine()) != null)
                {
                    temp.Add(line);
                }
                return temp;
            }
            else return null;
        }

        public List<string> Build()
        {
            List<string> pics = new List<string>();
            List<string> temp;
            pics.Add(".png");
            pics.Add(".jpg");
            pics.Add(".jpeg");
            pics.Add(".bmp");

            if (!finalized)
            {
                temp = AddPremable();
                if (temp != null)
                {
                    _finLines = temp;
                }

                foreach (MyFile f in _fileList)
                {
                    if (f.Extension == ".pdf")
                    {
                        _finLines.Add(AddPDF(f));
                    }
                    else if (pics.Contains(f.Extension.ToLower()))
                    {
                        _finLines.Add(AddImage(f));
                    }
                    else
                    {
                        _finLines.Add(AddCode(f));
                    }
                }

                Finalize();
            }
            return _finLines;
        }

        public string AddImage(MyFile f)
        {
            return "";
        }

        public string AddPDF(MyFile f)
        {
            return "";
        }

        public string AddCode(MyFile f)
        {
            return "";
        }

        public TexMaker Finalize()
        {
            if (finalized)
                return this;
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

            return missingFiles;
        }
    }
}