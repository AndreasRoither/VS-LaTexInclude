
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LatechInclude.HelperClasses;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace LatechInclude.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public ICommand PathFolderDialogCommand { get; private set; }
        public ICommand TexMakerCommand { get; private set; }
        public ICommand SettingsCommand { get; private set; }

        private CommonOpenFileDialog dlg = new CommonOpenFileDialog();
        private static TrulyObservableCollection<MyFile> _fileList = new TrulyObservableCollection<MyFile>();
        private List<string> _Languages = new List<string>();

        public static string currentLanguage = null;
        private readonly string regexPattern = @"\$(.*?)\$";

        public List<string> WhiteList = new List<string>();

        public MainViewModel() 
        {
            PathFolderDialogCommand = new RelayCommand(PathFolderDialogMethod);
            TexMakerCommand = new RelayCommand(TexMakerMethod);
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

                foreach (string s in WhiteListLines)
                {
                    if (s.StartsWith("."))
                    {
                        WhiteList.Add(s);
                    }
                }

                //string line;

                //// Read the file and display it line by line.
                //System.IO.StreamReader file =
                //   new System.IO.StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\Languages.txt"));
                //while ((line = file.ReadLine()) != null)
                //{
                //    _Languages.Add(new Languages(line));
                //}

                //file.Close();

                //_Languages = System.IO.File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\Languages.txt"));
                foreach (string s in WhiteListLines)
                {
                    if (s.StartsWith("#"))
                    {               
                        _Languages.Add(s.Remove(0, 1));
                    }
                }


            }
            catch(Exception ex)
            { }

            Console.WriteLine();
        }

        public override void Cleanup()
        {
            PathFolderDialogCommand = null;
            TexMakerCommand = null;
            SettingsCommand = null;
            dlg = null;

            base.Cleanup();
        }

        public List<string> Languages
        {
            get { return _Languages; }
            set { _Languages = value; }
        }

        public TrulyObservableCollection<MyFile> List
        {
            get { return _fileList; }
            set { _fileList = value; }
        }
        
        public void PathFolderDialogMethod()
        {
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folder = dlg.FileName;
                //Throws exception if no permissions
                try
                {
                    string[] files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
                    _fileList.Clear();

                    int i = 1;
                    foreach (string file in files)
                    {
                        if (WhiteList.Contains(System.IO.Path.GetExtension(file)))
                        {
                            _fileList.Add(new MyFile(System.IO.Path.GetFileNameWithoutExtension(file), file, System.IO.Path.GetExtension(file), i));
                            i++;
                        }
                        
                    }
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
                        if (currentLanguage == null)
                        {
                            fields.Add("Language", "Test");
                        }
                        else
                        {
                            fields.Add("Language", currentLanguage);
                        }
                        
                        fields.Add("Path", file.Path);
                        outputString += re.Replace(TexCodeTemplate, delegate (Match match)
                        {
                            return fields[match.Groups[1].Value];
                        });
                        outputString += "\n";
                        fields.Clear();
                    }

                    System.IO.File.WriteAllText((System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\output.tex"), outputString);

                    Console.WriteLine(outputString);
                }
                catch(Exception ex)
                { }
            }
            else
            {
                
            }

        }

        public void SettingsMethod()
        {
            SwitchViewWindow svw = new SwitchViewWindow();
            svw.DataContext = new SettingsViewModel();

            svw.ShowDialog();
        }
    }
}