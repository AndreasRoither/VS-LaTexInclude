using System;
using LatechInclude.HelperClasses;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LatechInclude.ViewModel;
using MahApps.Metro.Controls;

namespace LatechInclude
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        ObservableCollection<MyFile> _empList = new ObservableCollection<MyFile>();

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();

            //Style itemContainerStyle = new Style(typeof(ListBoxItem));
            //itemContainerStyle.Setters.Add(new Setter(ListBoxItem.AllowDropProperty, true));
            //itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.PreviewMouseMoveEvent, new MouseEventHandler(s_PreviewMouseLeftButtonDown)));
            //itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.DropEvent, new DragEventHandler(listbox1_Drop)));
            //MainListView.ItemContainerStyle = itemContainerStyle;

            this.DataContext = new MainViewModel();

        }

        void s_PreviewMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            MainViewModel mw = new MainViewModel();
            _empList = mw.List;

            if (sender is ListBoxItem && e.LeftButton == MouseButtonState.Pressed)
            {
                ListBoxItem draggedItem = sender as ListBoxItem;
                DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                draggedItem.IsSelected = true;
            }
        }

        //void listbox1_Drop(object sender, DragEventArgs e)
        //{
        //    MainViewModel mw = new MainViewModel();
        //    _empList = mw.List;

        //    MyFile droppedData = e.Data.GetData(typeof(MyFile)) as MyFile;
        //    MyFile target = ((ListBoxItem)(sender)).DataContext as MyFile;

        //    int removedIdx = MainListView.Items.IndexOf(droppedData);
        //    int targetIdx = MainListView.Items.IndexOf(target);

        //    if (removedIdx < targetIdx)
        //    {
        //        _empList.Insert(targetIdx + 1, droppedData);
        //        _empList.RemoveAt(removedIdx);
        //    }
        //    else
        //    {
        //        int remIdx = removedIdx + 1;
        //        if (_empList.Count + 1 > remIdx)
        //        {
        //            _empList.Insert(targetIdx, droppedData);
        //            _empList.RemoveAt(remIdx);
        //        }
        //    }

        //    int i = 1;
        //    foreach (MyFile file in _empList)
        //    {
        //        file.Position = i;
        //        i++;
        //    }
        //    mw.List = _empList;
        //    MainListView.Items.Refresh();
        //}

        //private void makeTex_Click(object sender, RoutedEventArgs e)
        //{
        //    TexMaker t = new TexMaker();

        //    Console.WriteLine("QQQQ" + t.getContent());
        //}
    }
}