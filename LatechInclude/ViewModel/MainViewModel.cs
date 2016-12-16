
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LatechInclude.HelperClasses;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace LatechInclude.ViewModel 
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Inherits from INotifyPropertyChanged, ViewModelBase
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public ICommand PathFolderDialogCommand { get; private set; }
        public ICommand TexMakerCommand { get; private set; }
        public ICommand AddExtensionCommand { get; private set; }
        public ICommand SettingsCommand { get; private set; }
         
        public event PropertyChangedEventHandler PropertyChanged;

        public static string _currentLanguage;
        private static TrulyObservableCollection<MyFile> _fileList = null;
        private static List<WhiteList> _whiteList = null;
        private static List<WhiteList> _currentWhiteList = null;
        private static List<string> _Languages = null;

        private CommonOpenFileDialog dlg = new CommonOpenFileDialog();

        private string _statusText = null;
        private StringNotify pathString = null;
  
        private readonly string regexPattern;
        private readonly string regexReplacePattern;

        public MainViewModel() 
        {
            currentLanguage = "";
            _fileList = new TrulyObservableCollection<MyFile>();

            _statusText = "";
            pathString = new StringNotify("");
            _Languages = new List<string>();
            _whiteList = new List<WhiteList>();
            _currentWhiteList = new List<WhiteList>();

            regexPattern = @"\$(.*?)\$";
            regexReplacePattern = @"[\\]";

            PathFolderDialogCommand = new RelayCommand(PathFolderDialogMethod);
            TexMakerCommand = new RelayCommand(TexMakerMethod);
            AddExtensionCommand = new RelayCommand(AddExtensionMethod);
            SettingsCommand = new RelayCommand(SettingsMethod);

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

            try
            {
                string[] WhiteListLines = System.IO.File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\WhiteList.txt"));

                Init_WhiteList(WhiteListLines);           
            }
            catch(Exception ex)
            {

            }

            Console.WriteLine();
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
                Console.WriteLine(propertyName);
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
            set { _whiteList = value; }
        }

        public List<WhiteList> currentWhiteList
        {
            get { return _currentWhiteList; }
            set { _currentWhiteList = value; }
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
                _statusText = value;
                OnPropertyChanged("statusText");
            }
        }      

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
                        foreach(WhiteList wl in _currentWhiteList)
                        {
                            if(wl.Extension == Path.GetExtension(file))
                            {
                                _fileList.Add(new MyFile(System.IO.Path.GetFileNameWithoutExtension(file), file, System.IO.Path.GetExtension(file), i));
                                i++;
                            }
                        } 
                    }

                    statusText = (i-1) + " Files found";
                }
                catch(Exception ex)
                { }
            }
        }

        public void TexMakerMethod()
        {
            MainWindow mw = new MainWindow();

            if (_fileList.Count > 0)
            {
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

                    foreach (MyFile file in _fileList)
                    {
                        if (_currentLanguage == "")
                        {
                            fields.Add("Language", "Test");
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

                    System.IO.File.WriteAllText((System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\output.tex"), outputString);
                    statusText = "Tex successful";

                    Console.WriteLine(outputString);
                }
                catch(Exception ex)
                {
                    statusText = "Exception in Tex";
                }
            }
            else
            {
                statusText = @"Nothing to write ¯\_(ツ)_/¯";
            }
        }

        public void AddExtensionMethod()
        {

            SwitchViewWindow svw = new SwitchViewWindow();
            AddExtensionViewModel aevm = new AddExtensionViewModel();
            svw.DataContext = aevm;
            svw.Title = "Add Extension";
            svw.Owner = Application.Current.MainWindow;
            svw.ShowDialog();
        }

        public void SettingsMethod()
        {
            SwitchViewWindow svw = new SwitchViewWindow();
            svw.DataContext = new SettingsViewModel();


            svw.ShowDialog();
        }

        public void Init_WhiteList(string[] WhiteListLines)
        {
            WhiteList wl = new WhiteList();

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

                    _whiteList.Add(new WhiteList {
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

        public void clearCurrentWhiteList()
        {
            _currentWhiteList.Clear();
        }
    }
}