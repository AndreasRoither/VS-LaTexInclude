using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace LaTexInclude.HelperClasses
{
    public class TexMaker
    {
        private Boolean _missingFiles = false;
        private Boolean finalized = false;
        private string TexCodeTemplate = "";
        private string TexImageTemplate = "";
        private string TexPDFTemplate = "";
        private string _errorMsg = "";
        private Regex re;
        private Regex reg;
        private String regexPattern = @"\$(.*?)\$";
        private String regexReplacePattern = @"[\\]";
        private readonly string assemblyPath;
        private List<string> _finLines;
        private TrulyObservableCollection<MyFile> _fileList;
        private List<WhiteList> _whiteList;
        private int _processedFiles;
        private int _faultyFiles;

        /// <summary>
        /// TexMaker constructor
        /// </summary>
        /// <param name="fileList">List which contains the files</param>
        /// <param name="whiteList">List which contains the exntensions + language</param>
        public TexMaker()
        {
            _finLines = new List<string>();
            _processedFiles = 0;
            _faultyFiles = 0;

            assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            re = new Regex(regexPattern, RegexOptions.Compiled);
            reg = new Regex(regexReplacePattern);

            if (!LoadFiles())
            {
                _missingFiles = true;
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

        /// <summary>
        /// Gets processedFiles
        /// </summary>
        public int ProcessedFiles
        {
            get
            {
                return _processedFiles;
            }
        }

        /// <summary>
        /// Gets faultyFiles
        /// </summary>
        public int FaultyFiles
        {
            get
            {
                return _faultyFiles;
            }
        }

        /// <summary>
        /// Get the ErrorMsg
        /// </summary>
        public string ErrorMsg
        {
            get { return _errorMsg; }
            set { }
        }

        /// <summary>
        /// Gets or sets the filelist
        /// </summary>
        public TrulyObservableCollection<MyFile> FileList
        {
            get
            {
                return _fileList;
            }
            set
            {
                _fileList = value;
            }
        }

        /// <summary>
        /// Gets or sets the whitelist
        /// </summary>
        public List<WhiteList> WhiteList
        {
            get
            {
                return _whiteList;
            }
            set
            {
                _whiteList = value;
            }
        }

        /// <summary>
        /// Add pre text to the .tex
        /// </summary>
        /// <returns></returns>
        private String AddPremable()
        {
            if (File.Exists(Path.Combine(assemblyPath, @"Resources\Premable.tex")) | File.Exists(Path.Combine(assemblyPath, @"Resources\Premable.txt")))
            {
                return System.IO.File.ReadAllText(Path.Combine(assemblyPath, @"Resources\TexCodeTemplate.tex"));
            }
            else return null;
        }

        /// <summary>
        /// Add finish to tex lines
        /// </summary>
        /// <returns></returns>
        private String AddFinish()
        {
            if (File.Exists(Path.Combine(assemblyPath, @"Resources\Premable.tex")) | File.Exists(Path.Combine(assemblyPath, @"Resources\Premable.txt")))
            {
                return System.IO.File.ReadAllText(Path.Combine(assemblyPath, @"Resources\TexCodeTemplate.tex"));
            }
            else return null;
        }

        /// <summary>
        /// Build the tex
        /// </summary>
        /// <returns>List with all the tex lines</returns>
        public List<string> Build()
        {
            if (_fileList.Count > 0 && _whiteList.Count > 0)
            {
                List<string> pics = new List<string>();
                _finLines.Clear();
                _faultyFiles = 0;
                _processedFiles = 0;
                _errorMsg = "";

                string temp;
                pics.Add(".png");
                pics.Add(".jpg");
                pics.Add(".jpeg");
                pics.Add(".bmp");

                temp = AddPremable();
                if (temp != null)
                    _finLines.Add(temp);

                foreach (MyFile f in _fileList)
                {
                    if (f.Path.Contains(" "))
                    {
                        _faultyFiles++;
                        _errorMsg = _faultyFiles + " File path(s) have whitespaces in them, compiling will result in an error";
                    }

                    if (f.Extension == ".pdf")
                        _finLines.Add(AddPDF(f));
                    else if (pics.Contains(f.Extension.ToLower()))
                        _finLines.Add(AddImage(f));
                    else
                        _finLines.Add(AddCode(f));

                    _processedFiles++;
                }
                Finalize();
                return _finLines;
            }
            if (_fileList.Count == 0)
                _errorMsg = "FileList is empty";
            else
                _errorMsg = "WhiteList is empty";
            return null;
        }

        /// <summary>
        /// Changes the image texcode template and returns it
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public string AddImage(MyFile f)
        {
            StringDictionary fields = new StringDictionary();
            string temp = f.Path;

            temp = reg.Replace(temp, "/");
            fields.Add("Path", temp);
            string output = "";

            output += re.Replace(TexImageTemplate, delegate (Match match)
            {
                return fields[match.Groups[1].Value];
            });

            output += "\n";
            fields.Clear();
            return output;
        }

        /// <summary>
        /// Changes the pdf tex template and returns it
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public string AddPDF(MyFile f)
        {
            StringDictionary fields = new StringDictionary();
            string temp = f.Path;

            temp = reg.Replace(temp, "/");
            fields.Add("Path", temp);
            string output = "";

            output += re.Replace(TexPDFTemplate, delegate (Match match)
            {
                return fields[match.Groups[1].Value];
            });

            output += "\n";
            fields.Clear();
            return output;
        }

        /// <summary>
        /// Changes the code tex template and returns it
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public string AddCode(MyFile f)
        {
            StringDictionary fields = new StringDictionary();
            string temp = f.Path;

            foreach (WhiteList wl in _whiteList)
            {
                if (f.Extension == wl.Extension)
                {
                    fields.Add("Language", wl.Language);
                    break;
                }
            }

            temp = reg.Replace(temp, "/");
            fields.Add("Path", temp);
            string output = "";

            output += re.Replace(TexCodeTemplate, delegate (Match match)
            {
                return fields[match.Groups[1].Value];
            });

            output += "\n";
            fields.Clear();
            return output;
        }

        /// <summary>
        /// Finalize the tex code
        /// </summary>
        /// <returns></returns>
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
            bool missingFiles = true;

            if (File.Exists(Path.Combine(assemblyPath, @"Resources\TexCodeTemplate.tex")))
            {
                TexCodeTemplate = System.IO.File.ReadAllText(Path.Combine(assemblyPath, @"Resources\TexCodeTemplate.tex"));
            }
            else
            {
                string exampleLine = "\\lstinputlisting[language=$Language$] {$Path$}";

                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter((assemblyPath + "\\Resources\\TexCodeTemplate.tex")))
                {
                    file.WriteLine(exampleLine);
                }
                missingFiles = true;
                TexCodeTemplate = exampleLine;
            }

            if (File.Exists(Path.Combine(assemblyPath, @"Resources\TexImageTemplate.tex")))
            {
                TexImageTemplate = System.IO.File.ReadAllText(Path.Combine(assemblyPath, @"Resources\TexImageTemplate.tex"));
            }
            else
            {
                string exampleLine = "\\begin{figure}[H]\n\\centering\n\\includegraphics[scale=0.75]{$Path$}\n" +
                    "\\caption{ Caption }\n\\label{ fig: Label }\n\\end{figure}";

                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter((assemblyPath + "\\Resources\\TexImageTemplate.tex")))
                {
                    file.WriteLine(exampleLine);
                }
                missingFiles = true;
                TexImageTemplate = exampleLine;
            }

            if (File.Exists(Path.Combine(assemblyPath, @"Resources\TexPDFTemplate.tex")))
            {
                TexPDFTemplate = System.IO.File.ReadAllText(Path.Combine(assemblyPath, @"Resources\TexPDFTemplate.tex"));
            }
            else
            {
                string exampleLine = "\\includepdf[pages=1-,pagecommand={}]{$Path$}";

                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter((assemblyPath + "\\Resources\\TexPDFTemplate.tex")))
                {
                    file.WriteLine(exampleLine);
                }
                missingFiles = true;
                TexPDFTemplate = exampleLine;
            }
            return missingFiles;
        }
    }
}