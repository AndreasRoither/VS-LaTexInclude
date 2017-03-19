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
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

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
        /// Get the position of the element in the grid
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private delegate Point GetPosition(IInputElement element);

        /// <summary>
        /// row index standard value
        /// </summary>
        private int rowIndex = -1;

        /// <summary>
        /// current domain
        /// </summary>
        private AppDomain currentDomain;

        /// <summary>
        /// MainViewModel
        /// </summary>
        private MainViewModel _viewModel = new MainViewModel();

        /// <summary>
        /// Version number
        /// </summary>
        private int version = 112;

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

                    IconHelper.RemoveIcon(svw);

                    svw.ShowDialog();
                    svw = null;
                    uvm = null;

                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                }
            }

            InitializeComponent();

            //Add Global Exception Handling
            currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);

            Closing += (s, e) => ViewModelLocator.Cleanup();

            this.DataContext = this._viewModel;

            MainView_DataGrid.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(productsDataGrid_PreviewMouseLeftButtonDown);
            MainView_DataGrid.Drop += new System.Windows.DragEventHandler(MainView_DataGrid_Drop);
        }

        /// <summary>
        /// second constructor to avoid unnecessary memory
        /// </summary>
        /// <param name="b"></param>
        public MainWindow(bool b)
        {
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
        /// MainWindow finished loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            comboBox.SelectedIndex = 0;
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

        /// <summary>
        /// When an griditem is dropped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainView_DataGrid_Drop(object sender, System.Windows.DragEventArgs e)
        {
            _fileList = _viewModel.List;

            if (rowIndex < 0)
                return;
            int index = this.GetCurrentRowIndex(e.GetPosition);
            if (index < 0)
                return;
            if (index == rowIndex)
                return;
            if (index == MainView_DataGrid.Items.Count - 1)
            {
                System.Windows.MessageBox.Show("This row-index cannot be dropped");
                return;
            }

            MyFile tempFile = MainView_DataGrid.SelectedItem as MyFile;
            _fileList.RemoveAt(rowIndex);
            _fileList.Insert(index, tempFile);

            int i = 1;
            foreach (MyFile file in _fileList)
            {
                file.Position = i;
                i++;
            }
            _viewModel.List = _fileList;
            MainView_DataGrid.ItemsSource = null;
            MainView_DataGrid.ItemsSource = _viewModel.List;
        }

        /// <summary>
        /// Preview for the drag & drop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void productsDataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            rowIndex = GetCurrentRowIndex(e.GetPosition);
            if (rowIndex < 0)
                return;

            MainView_DataGrid.SelectedIndex = rowIndex;
            MyFile tempFile = MainView_DataGrid.Items[rowIndex] as MyFile;

            if (tempFile == null)
                return;

            System.Windows.DragDropEffects dragdropeffects = System.Windows.DragDropEffects.Move;
            if (DragDrop.DoDragDrop(MainView_DataGrid, tempFile, dragdropeffects)
                                != System.Windows.DragDropEffects.None)
            {
                MainView_DataGrid.SelectedItem = tempFile;
            }
        }

        /// <summary>
        /// Get Mouse row
        /// </summary>
        /// <param name="theTarget"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private bool GetMouseTargetRow(Visual theTarget, GetPosition position)
        {
            if (theTarget != null)
            {
                Rect rect = VisualTreeHelper.GetDescendantBounds(theTarget);
                Point point = position((IInputElement)theTarget);
                return rect.Contains(point);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the Datagrid row
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private DataGridRow GetRowItem(int index)
        {
            if (MainView_DataGrid.ItemContainerGenerator.Status
                    != GeneratorStatus.ContainersGenerated)
                return null;
            return MainView_DataGrid.ItemContainerGenerator.ContainerFromIndex(index)
                                                            as DataGridRow;
        }

        /// <summary>
        /// Gets current row index
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private int GetCurrentRowIndex(GetPosition pos)
        {
            int curIndex = -1;
            for (int i = 0; i < MainView_DataGrid.Items.Count; i++)
            {
                DataGridRow itm = GetRowItem(i);
                if (GetMouseTargetRow(itm, pos))
                {
                    curIndex = i;
                    break;
                }
            }
            return curIndex;
        }

        /// <summary>
        /// Column header on click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void columnHeader_Click(object sender, RoutedEventArgs e)
        {
            var columnHeader = sender as DataGridColumnHeader;
            if (columnHeader != null)
            {
                int i;

                switch (columnHeader.Content.ToString())
                {
                    case "Name":

                        _fileList = _viewModel.List;
                        _fileList = new TrulyObservableCollection<MyFile>(from file in _fileList orderby file.FileName select file);

                        i = 1;
                        foreach (MyFile file in _fileList)
                        {
                            file.Position = i;
                            i++;
                        }

                        _viewModel.List = _fileList;
                        MainView_DataGrid.ItemsSource = null;
                        MainView_DataGrid.ItemsSource = _viewModel.List;

                        break;

                    case "Extension":

                        _fileList = _viewModel.List;
                        _fileList = new TrulyObservableCollection<MyFile>(from file in _fileList orderby file.Extension select file);

                        i = 1;
                        foreach (MyFile file in _fileList)
                        {
                            file.Position = i;
                            i++;
                        }

                        _viewModel.List = _fileList;
                        MainView_DataGrid.ItemsSource = null;
                        MainView_DataGrid.ItemsSource = _viewModel.List;
                        break;
                }
            }
        }

        /// <summary>
        /// Saves the new selected item in the viewmodel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnLanguageBox_Changed(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.CurrentLanguage = (sender as ComboBox).SelectedItem as string;
            WhiteList_Grid.ItemsSource = null;

            if (_viewModel.CurrentLanguage == "All")
            {
                _viewModel.CurrentWhiteList = _viewModel.WhiteList;
            }
            else
            {
                List<WhiteList> tempList = new List<WhiteList>();

                foreach (WhiteList wl in _viewModel.WhiteList)
                {
                    if (wl.Language == _viewModel.CurrentLanguage)
                    {
                        tempList.Add(new WhiteList
                        {
                            Language = wl.Language,
                            Extension = wl.Extension
                        });
                    }
                }

                _viewModel.CurrentWhiteList = tempList;
            }
            WhiteList_Grid.ItemsSource = _viewModel.CurrentWhiteList;
        }

        /// <summary>
        /// Removes a single item from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRemoveItemClick_Main(object sender, RoutedEventArgs e)
        {
            if (MainView_DataGrid.SelectedItems.Count != 0)
            {
                int count = 1;
                string fileName;
                MyFile row = (MyFile)MainView_DataGrid.SelectedItems[0];

                if (row.FileName.Length > 20)
                {
                    fileName = row.FileName.Substring(0, 20) + row.Extension;
                }
                else
                {
                    fileName = row.FileName + row.Extension;
                }

                _viewModel.NotifyMessage = "Removed " + fileName;
                _viewModel.List.Remove(row);

                foreach (MyFile file in _viewModel.List)
                {
                    file.Position = count;
                    count++;
                }

                _viewModel.FlyoutOpen = true;
                MainView_DataGrid.ItemsSource = null;
                MainView_DataGrid.ItemsSource = _viewModel.List;

                fileName = null;
                row = null;
            }
            else
            {
                _viewModel.NotifyMessage = "No Item selected";
                _viewModel.FlyoutOpen = true;
            }
        }

        /// <summary>
        /// Removes all extensions from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRemoveExtensions_Main(object sender, RoutedEventArgs e)
        {
            if (MainView_DataGrid.SelectedItems.Count != 0)
            {
                int count = 1;
                MyFile row = (MyFile)MainView_DataGrid.SelectedItems[0];
                List<MyFile> temp = new List<MyFile>();

                foreach (MyFile file in _viewModel.List)
                {
                    if (file.Extension != row.Extension)
                    {
                        file.Position = count;
                        count++;
                    }
                    else
                    {
                        temp.Add(file);
                    }
                }

                foreach (MyFile file in temp)
                {
                    _viewModel.List.Remove(file);
                }

                _viewModel.NotifyMessage = "Removed all with " + row.Extension + " extension";
                _viewModel.FlyoutOpen = true;
                MainView_DataGrid.ItemsSource = null;
                MainView_DataGrid.ItemsSource = _viewModel.List;

                row = null;
                temp = null;
            }
            else
            {
                _viewModel.NotifyMessage = "No Item selected";
                _viewModel.FlyoutOpen = true;
            }
        }

        /// <summary>
        /// Clears the file list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClearClick__Main(object sender, RoutedEventArgs e)
        {
            _viewModel.List.Clear();
            _viewModel.NotifyMessage = "Cleared";
            _viewModel.FlyoutOpen = true;
            MainView_DataGrid.ItemsSource = null;
            MainView_DataGrid.ItemsSource = _viewModel.List;
        }

        /// <summary>
        /// Removes a single item from the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWhiteListRemoveClick_Main(object sender, RoutedEventArgs e)
        {
            if (WhiteList_Grid.SelectedItems.Count != 0)
            {
                int count = 0;
                WhiteList row = (WhiteList)WhiteList_Grid.SelectedItems[0];
                WhiteList temp = new WhiteList();

                _viewModel.NotifyMessage = "Removed " + row.Extension + " from " + row.Language;

                foreach (WhiteList w in _viewModel.WhiteList)
                {
                    if (w.Extension == row.Extension && w.Language == row.Language)
                    {
                        temp = w;
                    }

                    if (w.Language == row.Language)
                    {
                        count++;
                    }
                }

                if (count <= 1)
                {
                    _viewModel.Languages.Remove(row.Language);
                    comboBox.ItemsSource = null;
                    comboBox.ItemsSource = _viewModel.Languages;
                    comboBox.SelectedIndex = 0;
                }

                _viewModel.WhiteList.Remove(temp); ;
                _viewModel.CurrentWhiteList.Remove(row);

                _viewModel.FlyoutOpen = true;
                WhiteList_Grid.ItemsSource = null;
                WhiteList_Grid.ItemsSource = _viewModel.CurrentWhiteList;

                row = null;
                temp = null;
            }
            else
            {
                _viewModel.NotifyMessage = "No Item selected";
                _viewModel.FlyoutOpen = true;
            }
        }

        /// <summary>
        /// Clears the whitelist
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWhiteListClearClick_Main(object sender, RoutedEventArgs e)
        {
            if (comboBox.SelectedIndex == 0 || comboBox.SelectedIndex == -1)
            {
                _viewModel.WhiteList.Clear();
                _viewModel.ClearCurrentWhiteList();
                _viewModel.Languages.Clear();

                _viewModel.Languages.Add("All");
                comboBox.ItemsSource = null;
                comboBox.ItemsSource = _viewModel.Languages;
                comboBox.SelectedIndex = 0;
                _viewModel.NotifyMessage = "Cleared";
                _viewModel.FlyoutOpen = true;
                WhiteList_Grid.ItemsSource = null;
                WhiteList_Grid.ItemsSource = _viewModel.WhiteList;
            }
            else
            {
                List<WhiteList> temp = new List<WhiteList>();
                string language = comboBox.SelectedItem.ToString();

                foreach (WhiteList w in _viewModel.WhiteList)
                {
                    if (w.Language == language)
                    {
                        temp.Add(w);
                    }
                }

                foreach (WhiteList w in temp)
                {
                    _viewModel.WhiteList.Remove(w);
                }

                _viewModel.CurrentWhiteList = _viewModel.WhiteList;
                _viewModel.Languages.Remove(language);

                comboBox.ItemsSource = null;
                comboBox.ItemsSource = _viewModel.Languages;
                WhiteList_Grid.ItemsSource = null;
                WhiteList_Grid.ItemsSource = _viewModel.CurrentWhiteList;
                comboBox.SelectedIndex = 0;

                temp = null;
                language = null;
            }
        }
    }
}