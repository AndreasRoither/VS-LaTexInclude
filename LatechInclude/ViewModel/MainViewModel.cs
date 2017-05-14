using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using LaTexInclude.HelperClasses;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace LaTexInclude.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Inherits from ViewModelBase, INotifyPropertyChanged
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public static SettingsViewModel svm_temp = new SettingsViewModel();

        public ICommand PathFolderDialogCommand { get; private set; }
        public ICommand TexMakerCommand { get; private set; }
        public ICommand AddExtensionCommand { get; private set; }
        public ICommand ExitCommand { get; private set; }

        new public event PropertyChangedEventHandler PropertyChanged;

        public static string _currentLanguage = "";
        private static TrulyObservableCollection<MyFile> _fileList = new TrulyObservableCollection<MyFile>();
        private static List<WhiteList> _whiteList = new List<WhiteList>();
        private static List<WhiteList> _currentWhiteList = new List<WhiteList>();
        private static List<string> _Languages = new List<string>();

        private CommonOpenFileDialog dlg = new CommonOpenFileDialog();

        private string _statusText = null;
        private StringNotify pathString = null;
        private bool isFlyoutOpen;
        private string _notifyMessage;

        private readonly string regexPattern;
        private readonly string regexReplacePattern;

        [PreferredConstructor]
        public MainViewModel()
        {
            PathFolderDialogCommand = new RelayCommand(PathFolderDialogMethod);
            TexMakerCommand = new RelayCommand(TexMakerMethod);
            AddExtensionCommand = new RelayCommand(AddExtensionMethod);
            ExitCommand = new RelayCommand(ExitMethod);

            _statusText = "";
            pathString = new StringNotify("");
            isFlyoutOpen = false;
            _notifyMessage = "";

            regexPattern = @"\$(.*?)\$";
            regexReplacePattern = @"[\\]";

            dlg.IsFolderPicker = true;

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            LoadFiles();
        }

        ~MainViewModel()
        {
            if (dlg != null)
            {
                dlg.Dispose();
                dlg = null;
            }
        }

        public override void Cleanup()
        {
            PathFolderDialogCommand = null;
            TexMakerCommand = null;
            AddExtensionCommand = null;
            ExitCommand = null;
            _fileList = null;
            _whiteList = null;
            _currentWhiteList = null;
            _Languages = null;

            base.Cleanup();
        }

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                this.PropertyChanged(this, e);
            }
        }

        /// <summary>
        /// Gets or sets CurrentLanguage
        /// </summary>
        public string CurrentLanguage
        {
            get { return _currentLanguage; }
            set { _currentLanguage = value; }
        }

        public TrulyObservableCollection<MyFile> List
        {
            get { return _fileList; }
            set { _fileList = value; }
        }

        /// <summary>
        /// Gets or sets WhiteList
        /// </summary>
        public List<WhiteList> WhiteList
        {
            get { return _whiteList; }
            set
            {
                _whiteList = value;
                OnPropertyChanged("WhiteList");
            }
        }

        /// <summary>
        /// Gets or sets CurrentWhiteList
        /// </summary>
        public List<WhiteList> CurrentWhiteList
        {
            get { return _currentWhiteList; }
            set
            {
                _currentWhiteList = value;
                OnPropertyChanged("CurrentWhiteList");
            }
        }

        /// <summary>
        /// Gets or sets FolderPath
        /// </summary>
        public string FolderPath
        {
            get { return pathString.text; }
            set
            {
                pathString.text = value;
                OnPropertyChanged("fPath");
            }
        }

        /// <summary>
        /// Gets or sets Languages
        /// </summary>
        public List<string> Languages
        {
            get { return _Languages; }
            set { _Languages = value; }
        }

        /// <summary>
        /// Gets or sets StatusText
        /// </summary>
        public string StatusText
        {
            get { return _statusText; }
            set
            {
                if (Properties.Settings.Default.Setting_General_StatusBar)
                {
                    _statusText = value;
                    OnPropertyChanged("statusText");
                }
            }
        }

        /// <summary>
        /// Gets or sets FlyoutOpen
        /// </summary>
        public bool FlyoutOpen
        {
            get { return isFlyoutOpen; }
            set
            {
                isFlyoutOpen = value;
                OnPropertyChanged("FlyoutOpen");
            }
        }

        /// <summary>
        /// Gets or sets FlyoutMessage
        /// </summary>
        public string NotifyMessage
        {
            get { return _notifyMessage; }
            set
            {
                if (_notifyMessage == value) return;
                _notifyMessage = value;
                OnPropertyChanged("NotifyMessage");
            }
        }

        /// <summary>
        /// Saving WhiteList
        /// </summary>
        public void Save()
        {
            List<WhiteList> tempWList = this.WhiteList;
            string outputString = "";
            string compareLanguage = "";

            foreach (WhiteList wl in tempWList)
            {
                if (compareLanguage != wl.Language)
                {
                    compareLanguage = wl.Language;
                    outputString += "#" + wl.Language;
                    outputString += Environment.NewLine;
                    outputString += wl.Extension;
                }
                else
                {
                    outputString += wl.Extension;
                }
                outputString += Environment.NewLine;
            }

            try
            {
                System.IO.File.WriteAllText((System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Resources\\WhiteList.txt"), outputString);
            }
            catch (Exception ex)
            {
                string temp_outputString = Environment.NewLine + "Exception caught" + Environment.NewLine + "Date: " + DateTime.UtcNow.Date.ToString("dd/MM/yyyy") + ", Time: " + DateTime.Now.ToString("HH:mm:ss tt") + Environment.NewLine + ex.Message + Environment.NewLine + ex.ToString() + Environment.NewLine;

                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\CrashLog.txt", true))
                {
                    file.WriteLine(temp_outputString);
                }

                temp_outputString = null;
            }
        }

        /// <summary>
        /// Loads an checks for missing files
        /// </summary>
        public void LoadFiles()
        {
            bool missingFiles = false;
            string message = "Missing ";
            string path_temp = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (!Directory.Exists(Path.Combine(path_temp, "\\Resources")))
            {
                Directory.CreateDirectory(path_temp + "\\Resources");
            }

            if (File.Exists(Path.Combine(path_temp, @"Resources\WhiteList.txt")))
            {
                string[] WhiteListLines = System.IO.File.ReadAllLines(Path.Combine(path_temp, @"Resources\WhiteList.txt"));

                Init_WhiteList(WhiteListLines);
            }
            else
            {
                string[] exampleLines = { "#C++", ".h", ".cpp", "#C", ".c", "#Pascal", ".pas", "#HTML", ".html", ".css", "#VHDL", ".vhdl" };

                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter((path_temp + "\\Resources\\WhiteList.txt")))
                {
                    foreach (string line in exampleLines)
                    {
                        file.WriteLine(line);
                    }
                }

                string[] WhiteListLines = System.IO.File.ReadAllLines(Path.Combine(path_temp, @"Resources\WhiteList.txt"));

                Init_WhiteList(WhiteListLines);
                message += "WhiteList ";
                missingFiles = true;
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
                NotifyMessage = message + "... added file(s)!";
                FlyoutOpen = true;
            }
        }

        /// <summary>
        /// Initialize the WhiteList
        /// </summary>
        /// <param name="WhiteListLines">string[] with lines containing the extension names</param>
        public void Init_WhiteList(string[] WhiteListLines)
        {
            if (_whiteList.Count == 0 && _Languages.Count == 0)
            {
                WhiteList wl = new WhiteList();
                _Languages.Add("All");

                foreach (string s in WhiteListLines)
                {
                    if (s.StartsWith("#"))
                    {
                        _Languages.Add(s.Remove(0, 1));
                        wl.Language = s.Remove(0, 1);
                    }

                    if (s.StartsWith("."))
                    {
                        wl.Extension = s;

                        _whiteList.Add(new WhiteList
                        {
                            Language = wl.Language,
                            Extension = wl.Extension
                        });

                        CurrentWhiteList.Add(new WhiteList
                        {
                            Language = wl.Language,
                            Extension = wl.Extension
                        });
                    }
                }
            }
        }

        /// <summary>
        /// Clears the currentWhiteList
        /// </summary>
        public void ClearCurrentWhiteList()
        {
            _currentWhiteList.Clear();
        }

        /// <summary>
        /// Opens up the path folder dialog
        /// </summary>
        public void PathFolderDialogMethod()
        {
            if (Properties.Settings.Default.Setting_General_UseCustomPath && Properties.Settings.Default.Setting_General_CustomPath != String.Empty && Properties.Settings.Default.Setting_General_CustomPath.Length >= 2)
            {
                dlg.InitialDirectory = Properties.Settings.Default.Setting_General_CustomPath;
            }
            else
            {
                dlg.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            }

            if (Properties.Settings.Default.Setting_General_UseRelativePath)
            {
                dlg.Title = "Select working directory";
                string workDirectory;
                string fileDirectory;

                if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    workDirectory = dlg.FileName;

                    dlg.Title = "Select file directory";

                    if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        fileDirectory = dlg.FileName;
                        SearchDirectories(fileDirectory, workDirectory);
                    }
                }
            }
            else
            {
                dlg.Title = "Folder Selection";

                if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var folder = dlg.FileName;
                    FolderPath = dlg.FileName;

                    SearchDirectories(folder);
                }
            }
        }

        /// <summary>
        /// Generates the string for the TextEditor window
        /// </summary>
        public void TexMakerMethod()
        {
            if (_fileList.Count > 0)
            {
                MainWindow mw = new MainWindow(true);

                Regex re = new Regex(regexPattern, RegexOptions.Compiled);
                Regex reg = new Regex(regexReplacePattern);
                string temp = "";

                try
                {
                    string TexCodeTemplate = System.IO.File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\TexCodeTemplate.tex"));
                    string TexImageTemplate = System.IO.File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\TexImageTemplate.tex"));
                    string TexPDFTemplate = System.IO.File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\TexPDFTemplate.tex"));

                    StringDictionary fields = new StringDictionary();

                    string output = re.Replace(TexCodeTemplate, delegate (Match match)
                    {
                        return fields[match.Groups[1].Value];
                    });

                    string outputString = "";
                    bool found;

                    foreach (MyFile file in _fileList)
                    {
                        found = false;
                        if (_currentLanguage == "" | _currentLanguage == "All")
                        {
                            foreach (WhiteList wl in CurrentWhiteList)
                            {
                                if (file.Extension == wl.Extension && !found)
                                {
                                    fields.Add("Language", wl.Language);
                                    found = true;
                                }
                            }

                            if (!found)
                            {
                                fields.Add("Language", "FillMe");
                            }
                        }
                        else
                        {
                            fields.Add("Language", _currentLanguage);
                        }

                        temp = file.Path;
                        temp = reg.Replace(temp, "/");
                        fields.Add("Path", temp);

                        outputString += re.Replace(TexCodeTemplate, delegate (Match match)
                        {
                            return fields[match.Groups[1].Value];
                        });
                        outputString += "\n";
                        fields.Clear();
                    }

                    TextEditorMethod(outputString);
                }
                catch (Exception ex)
                {
                    string outputString;
                    outputString = Environment.NewLine + "Exception caught" + Environment.NewLine + "Date: " + DateTime.UtcNow.Date.ToString("dd/MM/yyyy") + ", Time: " + DateTime.Now.ToString("HH:mm:ss tt") + Environment.NewLine + ex.Message + Environment.NewLine + ex.ToString() + Environment.NewLine;

                    using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\CrashLog.txt", true))
                    {
                        file.WriteLine(outputString);
                    }

                    outputString = null;
                }

                mw.Close();
                mw = null;
            }
            else
            {
                NotifyMessage = @"Nothing to write ¯\_(ツ)_/¯";
                FlyoutOpen = true;
            }
        }

        /// <summary>
        /// Shows the AddeExtensionView
        /// </summary>
        public void AddExtensionMethod()
        {
            AddExtensionViewModel aevm = new AddExtensionViewModel();

            SwitchViewWindow svw = new SwitchViewWindow()
            {
                DataContext = aevm,
                Title = "Add Extension",
                Height = 220,
                Width = 320,
            };

            svw.ShowDialog();

            if (svw.DialogResult == false)
            {
                CurrentWhiteList = CurrentWhiteList;
            }

            svw = null;
            aevm = null;
        }

        /// <summary>
        /// Shows the TxtEditorView
        /// </summary>
        /// <param name="outputString"></param>
        public void TextEditorMethod(string outputString)
        {
            TxtEditorViewModel tevm = new TxtEditorViewModel()
            {
                outputString = outputString
            };

            if (Properties.Settings.Default.Setting_General_CopyToClipboard)
            {
                outputString = outputString.Replace("\r\n", "\r");
                System.Windows.Forms.Clipboard.SetText(outputString);
                NotifyMessage = "Copied to clipboard";
                FlyoutOpen = true;
            }
            else
            {
                StartViewmodel.SelectedIndex = 1;
                NotifyMessage = "Success! Output copied to texteditor";
                FlyoutOpen = true;
            }

            tevm = null;
        }

        /// <summary>
        /// Closes the program
        /// </summary>
        public void ExitMethod()
        {
            if (Properties.Settings.Default.Setting_General_SaveWhiteList)
            {
                Save();
            }

            Environment.Exit(1);
        }

        /// <summary>
        /// Searches the directory and adds files to the File list
        /// </summary>
        /// <param name="folder"></param>
        public void SearchDirectories(string folder, string workdir = "")
        {
            //Throws exception if no permissions
            try
            {
                string[] files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
                _fileList.Clear();

                int i = 1;
                bool flag = false;
                foreach (string file in files)
                {
                    foreach (WhiteList wl in _currentWhiteList)
                    {
                        if (wl.Extension == Path.GetExtension(file))
                        {
                            if (Properties.Settings.Default.Setting_General_UseRelativePath)
                            {
                                string temp = MakeRelativePath(workdir + "\\", System.IO.Path.GetDirectoryName(file) + "\\");
                                temp += System.IO.Path.GetFileName(file);
                                _fileList.Add(new MyFile(System.IO.Path.GetFileNameWithoutExtension(file), temp, System.IO.Path.GetExtension(file), i));
                                i++;
                                flag = true;
                                break;
                            }
                            else
                            {
                                _fileList.Add(new MyFile(System.IO.Path.GetFileNameWithoutExtension(file), file, System.IO.Path.GetExtension(file), i));
                                i++;
                                flag = true;
                                break;
                            }
                        }
                    }
                    if (flag) continue;
                }

                StatusText = (i - 1) + " Files found";
            }
            catch (Exception ex)
            {
                string outputString;
                outputString = Environment.NewLine + "Exception caught" + Environment.NewLine + "Date: " + DateTime.UtcNow.Date.ToString("dd/MM/yyyy") + ", Time: " + DateTime.Now.ToString("HH:mm:ss tt") + Environment.NewLine + ex.Message + Environment.NewLine + ex.ToString() + Environment.NewLine;

                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\CrashLog.txt", true))
                {
                    file.WriteLine(outputString);
                }

                outputString = null;
            }
        }

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path or <c>toPath</c> if the paths are not related.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static String MakeRelativePath(String fromPath, String toPath)
        {
            if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }
    }
}