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
using System.Windows;
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
        public ICommand SettingsCommand { get; private set; }
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
            SettingsCommand = new RelayCommand(SettingsMethod);
            ExitCommand = new RelayCommand(ExitMethod);

            _statusText = "";
            pathString = new StringNotify("");
            isFlyoutOpen = false;
            _notifyMessage = "";

            regexPattern = @"\$(.*?)\$";
            regexReplacePattern = @"[\\]";

            dlg.Title = "Folder Selection";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = System.AppDomain.CurrentDomain.BaseDirectory;

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

        public override void Cleanup()
        {
            PathFolderDialogCommand = null;
            TexMakerCommand = null;
            SettingsCommand = null;
            dlg.Dispose();

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

        public string currentLanguage
        {
            get { return _currentLanguage; }
            set { _currentLanguage = value; }
        }

        public TrulyObservableCollection<MyFile> List
        {
            get { return _fileList; }
            set { _fileList = value; }
        }

        public List<WhiteList> whiteList
        {
            get { return _whiteList; }
            set
            {
                _whiteList = value;
                OnPropertyChanged("whiteList");
            }
        }

        public List<WhiteList> currentWhiteList
        {
            get { return _currentWhiteList; }
            set
            {
                _currentWhiteList = value;
                OnPropertyChanged("currentWhiteList");
            }
        }

        public string fPath
        {
            get { return pathString.text; }
            set
            {
                pathString.text = value;
                OnPropertyChanged("fPath");
            }
        }

        public List<string> Languages
        {
            get { return _Languages; }
            set { _Languages = value; }
        }

        public string statusText
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

        public bool FlyoutOpen
        {
            get { return isFlyoutOpen; }
            set
            {
                isFlyoutOpen = value;
                OnPropertyChanged("FlyoutOpen");
            }
        }

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
            List<WhiteList> tempWList = this.whiteList;
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

                        currentWhiteList.Add(new WhiteList
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
        public void clearCurrentWhiteList()
        {
            _currentWhiteList.Clear();
        }

        /// <summary>
        /// Opens up the path folder dialog
        /// </summary>
        public void PathFolderDialogMethod()
        {
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folder = dlg.FileName;
                fPath = dlg.FileName;

                //Throws exception if no permissions
                try
                {
                    string[] files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
                    _fileList.Clear();

                    int i = 1;
                    foreach (string file in files)
                    {
                        foreach (WhiteList wl in _currentWhiteList)
                        {
                            if (wl.Extension == Path.GetExtension(file))
                            {
                                _fileList.Add(new MyFile(System.IO.Path.GetFileNameWithoutExtension(file), file, System.IO.Path.GetExtension(file), i));
                                i++;
                            }
                        }
                    }

                    statusText = (i - 1) + " Files found";
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
                    bool found = false;

                    foreach (MyFile file in _fileList)
                    {
                        if (_currentLanguage == "" | _currentLanguage == "All")
                        {
                            foreach (WhiteList wl in currentWhiteList)
                            {
                                if (file.Extension == wl.Extension)
                                {
                                    found = true;
                                    fields.Add("Language", wl.Language);
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
            SwitchViewWindow svw = new SwitchViewWindow();
            AddExtensionViewModel aevm = new AddExtensionViewModel();
            svw.DataContext = aevm;
            svw.Title = "Add Extension";
            svw.Owner = Application.Current.MainWindow;
            svw.Height = 220;
            svw.Width = 320;

            svw.ShowDialog();

            if (svw.DialogResult == false)
            {
                OnPropertyChanged("currentWhiteList");
            }
            svw = null;
            aevm = null;
        }

        /// <summary>
        /// Shows the SettingsView
        /// </summary>
        public void SettingsMethod()
        {
            SwitchViewWindow svw = new SwitchViewWindow();
            //SettingsViewModel svm = new SettingsViewModel();
            svw.DataContext = svm_temp;
            svw.Title = "Settings";
            svw.Owner = Application.Current.MainWindow;
            svw.Height = 339;
            svw.Width = 426;

            svw.ShowDialog();
            svw = null;
        }

        /// <summary>
        /// Shows the TxtEditorView
        /// </summary>
        /// <param name="outputString"></param>
        public void TextEditorMethod(string outputString)
        {
            SwitchViewWindow svw = new SwitchViewWindow();
            TxtEditorViewModel tevm = new TxtEditorViewModel();
            tevm.outputString = outputString;
            svw.DataContext = tevm;
            svw.Title = "TextEditor";
            svw.Owner = Application.Current.MainWindow;
            svw.ResizeMode = ResizeMode.CanResize;

            svw.ShowDialog();
            svw = null;
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
    }
}