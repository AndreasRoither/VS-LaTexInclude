using LaTexInclude.HelperClasses;
using LaTexInclude.ViewModel;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace LaTexInclude.View
{
    /// <summary>
    /// Interaktionslogik für HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
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
        /// MainViewModel
        /// </summary>
        private MainViewModel _viewModel = new MainViewModel();

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public HomeView()
        {
            InitializeComponent();

            this.DataContext = this._viewModel;

            MainView_DataGrid.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(productsDataGrid_PreviewMouseLeftButtonDown);
            MainView_DataGrid.Drop += new System.Windows.DragEventHandler(MainView_DataGrid_Drop);
        }

        /// <summary>
        /// second constructor to avoid unnecessary memory
        /// </summary>
        /// <param name="b"></param>
        public HomeView(bool b)
        {
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