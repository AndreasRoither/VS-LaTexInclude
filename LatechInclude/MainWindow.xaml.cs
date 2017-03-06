
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Windows.Data;
using System.Diagnostics;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Security.Permissions;
using System.Data;
using LatechInclude.HelperClasses;
using LatechInclude.ViewModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace LatechInclude
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        TrulyObservableCollection<MyFile> _fileList = new TrulyObservableCollection<MyFile>();
        public delegate Point GetPosition(IInputElement element);
        int rowIndex = -1;

        AppDomain currentDomain;

        private MainViewModel _viewModel = new MainViewModel();

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public MainWindow()
        {
            //Check for multiple instances, kill old instance or kill the starting process
            if (System.Diagnostics.Process.GetProcessesByName(System.IO.Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location)).Count() > 1)
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

            InitializeComponent();

            //Add Global Exception Handling
            currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);

            Closing += (s, e) => ViewModelLocator.Cleanup();

            this.DataContext = this._viewModel;
            comboBox.SelectedIndex = 0;

            MainView_DataGrid.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(productsDataGrid_PreviewMouseLeftButtonDown);
            MainView_DataGrid.Drop += new System.Windows.DragEventHandler(MainView_DataGrid_Drop);

            string[] args = Environment.GetCommandLineArgs();

            string outputString;
            outputString = Environment.NewLine + "CommandLineArgs" + Environment.NewLine + "Date: " + DateTime.UtcNow.Date.ToString("dd/MM/yyyy") + ", Time: " + DateTime.Now.ToString("HH:mm:ss tt") + Environment.NewLine;

            foreach (string s in args)
            {
                outputString += s + Environment.NewLine;
            }

            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Args.txt", true))
            {
                file.WriteLine(outputString);
            }

            outputString = null;
        }

        /// <summary>
        /// Handler for exceptions, will be called if a exception happened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        static void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {

            Exception e = (Exception)args.ExceptionObject;
            Console.WriteLine("Exception caught : " + e.Message);
            Console.WriteLine("Runtime terminating: {0}", args.IsTerminating);

            string outputString;
            outputString = Environment.NewLine + "Exception caught" + Environment.NewLine + "Date: " + DateTime.UtcNow.Date.ToString("dd/MM/yyyy") + ", Time: " + DateTime.Now.ToString("HH:mm:ss tt") + Environment.NewLine + e.ToString() + Environment.NewLine + "Runtime terminating: " + args.IsTerminating;

            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\CrashLog.txt", true))
            {
                file.WriteLine(outputString);
            }

            outputString = null;
        }

        /// <summary>
        /// Saving WhiteList
        /// </summary>
        public void Save()
        {
            List<WhiteList> tempWList = _viewModel.whiteList;
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
                outputString = "";
                outputString = ex.ToString();
                System.IO.File.WriteAllText((System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\CrashLog.txt"), outputString);
                outputString = null;
            }
        }

        /// <summary>
        /// When an griditem is dropped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainView_DataGrid_Drop(object sender, System.Windows.DragEventArgs e)
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
        void productsDataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        private DataGridRow GetRowItem(int index)
        {
            if (MainView_DataGrid.ItemContainerGenerator.Status
                    != GeneratorStatus.ContainersGenerated)
                return null;
            return MainView_DataGrid.ItemContainerGenerator.ContainerFromIndex(index)
                                                            as DataGridRow;
        }

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

        private void columnHeader_Click(object sender, RoutedEventArgs e)
        {
            var columnHeader = sender as DataGridColumnHeader;
            if(columnHeader != null)
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
            _viewModel.currentLanguage = (sender as ComboBox).SelectedItem as string;
            WhiteList_Grid.ItemsSource = null;

            if (_viewModel.currentLanguage == "All")
            {
                _viewModel.currentWhiteList = _viewModel.whiteList;
            }
            else
            {
                List<WhiteList> tempList = new List<WhiteList>();

                foreach (WhiteList wl in _viewModel.whiteList)
                {
                    if (wl.Language == _viewModel.currentLanguage)
                    {
                        tempList.Add(new WhiteList
                        {
                            Language = wl.Language,
                            Extension = wl.Extension
                        });
                    }
                }

                _viewModel.currentWhiteList = tempList;
            }
            WhiteList_Grid.ItemsSource = _viewModel.currentWhiteList;
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
        }

        private void OnRemoveItemClick_Main(object sender, RoutedEventArgs e)
        {
            if (MainView_DataGrid.SelectedItems.Count != 0)
            {
                int count = 1;
                string fileName;
                MyFile row = (MyFile)MainView_DataGrid.SelectedItems[0];

                if(row.FileName.Length > 20)
                {
                    fileName = row.FileName.Substring(0, 20) + row.Extension;
                }
                else
                {
                    fileName = row.FileName + row.Extension;
                }
                
                _viewModel.NotifyMessage = "Removed " + fileName;
                _viewModel.List.Remove(row);
        
                foreach (MyFile file in _viewModel.List )
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

        private void OnClearClick__Main(object sender, RoutedEventArgs e)
        {
            _viewModel.List.Clear();
            _viewModel.NotifyMessage = "Cleared";
            _viewModel.FlyoutOpen = true;
            MainView_DataGrid.ItemsSource = null;
            MainView_DataGrid.ItemsSource = _viewModel.List;
        }

        private void OnWhiteListRemoveClick_Main(object sender, RoutedEventArgs e)
        {
            if (WhiteList_Grid.SelectedItems.Count != 0)
            {
                int count = 0;
                WhiteList row = (WhiteList)WhiteList_Grid.SelectedItems[0];
                WhiteList temp = new WhiteList();

                _viewModel.NotifyMessage = "Removed " + row.Extension + " from "+ row.Language;

                foreach(WhiteList w in _viewModel.whiteList)
                {
                    if(w.Extension == row.Extension && w.Language == row.Language)
                    {
                        temp = w;      
                    }

                    if(w.Language == row.Language)
                    {
                        count++;
                    }
                }

                if(count <= 1)
                {
                    _viewModel.Languages.Remove(row.Language);
                    comboBox.ItemsSource = null;
                    comboBox.ItemsSource = _viewModel.Languages;
                    comboBox.SelectedIndex = 0;
                }

                _viewModel.whiteList.Remove(temp); ;
                _viewModel.currentWhiteList.Remove(row);

                _viewModel.FlyoutOpen = true;
                WhiteList_Grid.ItemsSource = null;
                WhiteList_Grid.ItemsSource = _viewModel.currentWhiteList;

                row = null;
                temp = null;
            }
            else
            {
                _viewModel.NotifyMessage = "No Item selected";
                _viewModel.FlyoutOpen = true;
            }
        }

        private void OnWhiteListClearClick_Main(object sender, RoutedEventArgs e)
        {
            if (comboBox.SelectedIndex == 0 || comboBox.SelectedIndex == -1)
            {
                _viewModel.whiteList.Clear();
                _viewModel.clearCurrentWhiteList();
                _viewModel.Languages.Clear();

                _viewModel.Languages.Add("All");
                comboBox.ItemsSource = null;
                comboBox.ItemsSource = _viewModel.Languages;
                comboBox.SelectedIndex = 0;
                _viewModel.NotifyMessage = "Cleared";
                _viewModel.FlyoutOpen = true;
                WhiteList_Grid.ItemsSource = null;
                WhiteList_Grid.ItemsSource = _viewModel.whiteList;
            }
            else
            {
                List<WhiteList> temp = new List<WhiteList>();
                string language = comboBox.SelectedItem.ToString();

                foreach (WhiteList w in _viewModel.whiteList)
                {
                    if (w.Language == language)
                    {
                        temp.Add(w);
                    }
                }

                foreach (WhiteList w in temp)
                {
                    _viewModel.whiteList.Remove(w);
                }

                _viewModel.currentWhiteList = _viewModel.whiteList;
                _viewModel.Languages.Remove(language);

                comboBox.ItemsSource = null;
                comboBox.ItemsSource = _viewModel.Languages;
                WhiteList_Grid.ItemsSource = null;
                WhiteList_Grid.ItemsSource = _viewModel.currentWhiteList;
                comboBox.SelectedIndex = 0;

                temp = null;
                language = null;
            }
        }
    }
}