using LaTexInclude.HelperClasses;
using LaTexInclude.Model;
using LaTexInclude.ViewModel;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using System.Windows;

namespace LaTexInclude
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// contains the found files
        /// </summary>
        private TrulyObservableCollection<MyFile> _fileList = new TrulyObservableCollection<MyFile>();

        /// <summary>
        /// current domain
        /// </summary>
        private AppDomain currentDomain;

        /// <summary>
        /// MainViewModel
        /// </summary>
        private MainViewModel _viewModel = new MainViewModel();

        private StartViewmodel _start = new StartViewmodel();

        /// <summary>
        /// Version number
        /// </summary>
        private int version = 130;

        /// <summary>
        /// The parsed HTTPWebRequest from github
        /// </summary>
        private GetResponse responseObj = null;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public MainWindow()
        {
            //Add Global Exception Handling
            currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);

            //Check for multiple instances, kill old instance or kill the starting process
            if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 2)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
            else if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
            {
                MessageBoxResult result = MessageBox.Show("An Instance is already open, do you want to close it?", "Instance already open", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        var current = Process.GetCurrentProcess();
                        Process.GetProcessesByName(current.ProcessName).Where(t => t.Id != current.Id).ToList().ForEach(t => t.Kill());
                        break;

                    case MessageBoxResult.No:
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                        break;
                }
            }

            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                string[] temp = args.SubArray(1, args.Length - 1);

                int i = 1;
                _viewModel.List.Clear();

                foreach (string s in temp)
                {
                    if (Path.HasExtension(s))
                    {
                        _viewModel.List.Add(new MyFile(System.IO.Path.GetFileNameWithoutExtension(s), s, System.IO.Path.GetExtension(s), i));
                        i++;
                    }
                    else
                    {
                        string[] files = Directory.GetFiles(args[1], "*", SearchOption.AllDirectories);

                        foreach (string file in files)
                        {
                            foreach (WhiteList wl in _viewModel.CurrentWhiteList)
                            {
                                if (wl.Extension == Path.GetExtension(file))
                                {
                                    _viewModel.List.Add(new MyFile(System.IO.Path.GetFileNameWithoutExtension(file), file, System.IO.Path.GetExtension(file), i));
                                    i++;
                                }
                            }
                        }
                    }
                }

                _viewModel.StatusText = (i - 1) + " Files found";

                if (!Properties.Settings.Default.Setting_General_ContextStartup)
                {
                    if (Path.HasExtension(args[1]))
                    {
                        GenerateOutputTex(Path.GetDirectoryName(args[1]));
                    }
                    else
                    {
                        GenerateOutputTex(System.IO.Directory.GetParent(args[1]).FullName);
                    }
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
            }

            try
            {
                //GET /repos/:owner/:repo/releases/latest
                string url = "https://api.github.com/repos/AndiRoither/VS-LaTexInclude/releases/latest";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 6.1; WOW64; Trident/6.0;)";
                request.KeepAlive = false;
                request.Method = "GET";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    var encoding = ASCIIEncoding.ASCII;
                    using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                    {
                        var responseText = reader.ReadToEnd();
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        responseObj = (GetResponse)js.Deserialize(responseText, typeof(GetResponse));
                    }
                }
            }
            catch (Exception e)
            {
                string outputString;
                outputString = Environment.NewLine + "Exception caught" + Environment.NewLine + "Date: " + DateTime.UtcNow.Date.ToString("dd/MM/yyyy") + ", Time: " + DateTime.Now.ToString("HH:mm:ss tt") + e.Message + Environment.NewLine + Environment.NewLine + e.ToString() + Environment.NewLine;

                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\CrashLog.txt", true))
                {
                    file.WriteLine(outputString);
                }

                outputString = null;
            }

            if (responseObj != null && responseObj.Prerelease != true && responseObj.Draft != true)
            {
                string tag_name = String.Copy(responseObj.Tag_name);
                tag_name = tag_name.Remove(0, 1);
                tag_name = tag_name.Replace(".", string.Empty);

                int val = Int32.Parse(tag_name);
                if (val > version)
                {
                    UpdateViewModel uvm = new UpdateViewModel(this.responseObj);
                    SwitchViewWindow svw = new SwitchViewWindow()
                    {
                        DataContext = uvm,
                        Title = "New update for LaTexInclude"
                    };

                    svw.Height = 370;
                    svw.Width = 350;
                    svw.ShowDialog();
                    if (UpdateViewModel.LaterClicked)
                    {
                        svw = null;
                        uvm = null;
                    }
                    else
                    {
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
                    }
                }
            }

            InitializeComponent();

            Closing += (s, e) => ViewModelLocator.Cleanup();

            this.DataContext = this._start;
            StartViewmodel.SelectedIndex = 0;
            //HamburgerMenuControl.SelectedIndex = 0;
        }

        /// <summary>
        /// second constructor to avoid unnecessary memory
        /// </summary>
        /// <param name="b"></param>
        public MainWindow(bool b)
        {
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            HamburgerMenuControl.SelectedIndex = -1;
            HamburgerMenuControl.SelectedOptionsIndex = -1;
        }

        /// <summary>
        /// When the program is closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();

            if (Properties.Settings.Default.Setting_General_SaveWhiteList)
            {
                Save();
            }

            //Environment.Exit(0);
        }

        /// <summary>
        /// Handler for exceptions, will be called if a exception happened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;

            string outputString;
            outputString = Environment.NewLine + "Exception caught" + Environment.NewLine + "Date: " + DateTime.UtcNow.Date.ToString("dd/MM/yyyy") + ", Time: " + DateTime.Now.ToString("HH:mm:ss tt") + e.Message + Environment.NewLine + Environment.NewLine + e.ToString() + Environment.NewLine + "Runtime terminating: " + args.IsTerminating;

            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\CrashLog.txt", true))
            {
                file.WriteLine(outputString);
            }

            outputString = null;
        }

        /// <summary>
        /// Genereates output.tex if Setting_General_ContextStartup is not set
        /// </summary>
        /// <param name="path"></param>
        public void GenerateOutputTex(string path)
        {
            if (_viewModel.List.Count > 0)
            {
                Regex re = new Regex(@"\$(.*?)\$", RegexOptions.Compiled);
                Regex reg = new Regex(@"[\\]");
                string temp = "";

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

                foreach (MyFile file in _viewModel.List)
                {
                    foreach (WhiteList wl in _viewModel.CurrentWhiteList)
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

                try
                {
                    System.IO.File.WriteAllText((path + "\\output.tex"), outputString);
                }
                catch (Exception ex)
                {
                    string t;
                    t = Environment.NewLine + "Exception caught" + Environment.NewLine + "Date: " + DateTime.UtcNow.Date.ToString("dd/MM/yyyy") + ", Time: " + DateTime.Now.ToString("HH:mm:ss tt") + Environment.NewLine + ex.Message + Environment.NewLine + ex.ToString() + Environment.NewLine + "Runtime terminating: ";

                    using (System.IO.StreamWriter file =
                    new System.IO.StreamWriter(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\CrashLog.txt", true))
                    {
                        file.WriteLine(t);
                    }
                    t = null;
                }
            }
        }

        /// <summary>
        /// Saving WhiteList
        /// </summary>
        public void Save()
        {
            List<WhiteList> tempWList = _viewModel.WhiteList;
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
    }
}