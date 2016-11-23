
using LatechInclude.HelperClasses;
using LatechInclude.ViewModel;
using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;

namespace LatechInclude
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        TrulyObservableCollection<MyFile> _fileList = new TrulyObservableCollection<MyFile>();
        MainViewModel mw = new MainViewModel();
        public delegate Point GetPosition(IInputElement element);
        int rowIndex = -1;

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();

            this.DataContext = new MainViewModel();

            MainView_DataGrid.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(productsDataGrid_PreviewMouseLeftButtonDown);
            MainView_DataGrid.Drop += new System.Windows.DragEventHandler(MainView_DataGrid_Drop);

        }

        void MainView_DataGrid_Drop(object sender, System.Windows.DragEventArgs e)
        {
            _fileList = mw.List;

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
            mw.List = _fileList;
            MainView_DataGrid.ItemsSource = null;
            MainView_DataGrid.ItemsSource = mw.List;
        }

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

                        _fileList = mw.List;
                        _fileList = new TrulyObservableCollection<MyFile>(from file in _fileList orderby file.FileName select file);

                        i = 1;
                        foreach (MyFile file in _fileList)
                        {
                            file.Position = i;
                            i++;
                        }

                        mw.List = _fileList;
                        MainView_DataGrid.ItemsSource = null;
                        MainView_DataGrid.ItemsSource = mw.List;

                        break;
                    case "Extension":
                        
                        _fileList = mw.List;
                        _fileList = new TrulyObservableCollection<MyFile>(from file in _fileList orderby file.Extension select file);

                        i = 1;
                        foreach (MyFile file in _fileList)
                        {
                            file.Position = i;
                            i++;
                        }

                        mw.List = _fileList;
                        MainView_DataGrid.ItemsSource = null;
                        MainView_DataGrid.ItemsSource = mw.List;
                        break;
                }
            }

        }


        //private void makeTex_Click(object sender, RoutedEventArgs e)
        //{
        //    TexMaker t = new TexMaker();

        //    Console.WriteLine("QQQQ" + t.getContent());
        //}
    }
}