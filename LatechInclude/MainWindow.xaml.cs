using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Threading;
using System.ComponentModel;
using System.IO;
using System.Collections.ObjectModel;

namespace LatechInclude
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CommonOpenFileDialog dlg = new CommonOpenFileDialog();
        ObservableCollection<MyFile> _empList = new ObservableCollection<MyFile>();

        public MainWindow()
        {
            InitializeComponent();

            Style itemContainerStyle = new Style(typeof(ListBoxItem));
            itemContainerStyle.Setters.Add(new Setter(ListBoxItem.AllowDropProperty, true));
            itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.PreviewMouseMoveEvent, new MouseEventHandler(s_PreviewMouseLeftButtonDown)));
            itemContainerStyle.Setters.Add(new EventSetter(ListBoxItem.DropEvent, new DragEventHandler(listbox1_Drop)));
            ListView.ItemContainerStyle = itemContainerStyle;


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
        }

        private void MainWindow_onPathclick(object sender, RoutedEventArgs e)
        {   
            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folder = dlg.FileName;
                path_TextBox.Text = folder;
      
                _empList.Clear();
                ListView.ItemsSource = null;
                ListView.Items.Clear();

                string[] files = Directory.GetFiles(dlg.FileName);

                int i = 1;
                foreach (string file in files)
                {
                    _empList.Add(new MyFile(System.IO.Path.GetFileNameWithoutExtension(file), file, System.IO.Path.GetExtension(file),i));
                    i++;
                }
                ListView.ItemsSource = _empList;
                ListView.Items.Refresh();

            }
        }

        private void ListView_OnNameClick(object sender, RoutedEventArgs e)
        {
            _empList = new ObservableCollection<MyFile>(from file in _empList orderby file.FileName select file);
            int i = 1;
            foreach (MyFile file in _empList)
            {
                file.Position = i;
                i++;
            }
            ListView.ItemsSource = _empList;
            ListView.Items.Refresh();
        }

        private void ListView_OnEndingClick(object sender, RoutedEventArgs e)
        {
            _empList = new ObservableCollection<MyFile>(from file in _empList orderby file.Extension select file);
            int i = 1;
            foreach (MyFile file in _empList)
            {
                file.Position = i;
                i++;
            }
            ListView.ItemsSource = _empList;
            ListView.Items.Refresh();
        }

        void s_PreviewMouseLeftButtonDown(object sender, MouseEventArgs e)
        {

            if (sender is ListBoxItem && e.LeftButton == MouseButtonState.Pressed)
            {
                ListBoxItem draggedItem = sender as ListBoxItem;
                DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
                draggedItem.IsSelected = true;
            }
        }

        void listbox1_Drop(object sender, DragEventArgs e)
        {
            MyFile droppedData = e.Data.GetData(typeof(MyFile)) as MyFile;
            MyFile target = ((ListBoxItem)(sender)).DataContext as MyFile;

            int removedIdx = ListView.Items.IndexOf(droppedData);
            int targetIdx = ListView.Items.IndexOf(target);

            if (removedIdx < targetIdx)
            {
                _empList.Insert(targetIdx + 1, droppedData);
                _empList.RemoveAt(removedIdx);
            }
            else
            {
                int remIdx = removedIdx + 1;
                if (_empList.Count + 1 > remIdx)
                {
                    _empList.Insert(targetIdx, droppedData);
                    _empList.RemoveAt(remIdx);
                }
            }

            int i = 1;
            foreach (MyFile file in _empList)
            {
                file.Position = i;
                i++;
            }
            ListView.Items.Refresh();
        }

        private void onMainWindow_File_Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void onMainWindow_File_Open_Click(object sender, RoutedEventArgs e)
        {

        }

        private void onMainWindow_File_Save_Click(object sender, RoutedEventArgs e)
        {
            statusBar_TextBlock.Text = "Saved";
        }

        private void onTextEditor_Click(object sender, RoutedEventArgs e)
        {
            Window textEditor_Window = new TextEditorWindow();
            textEditor_Window.Show();
        }

        private void onMainWindow_Settings_Click(object sender, RoutedEventArgs e)
        {
            Window settings_Window = new SettingsWindow();
            settings_Window.Show();
        }

        public void setText(String text)
        {
            this.statusBar_TextBlock.Text = text;
        }
    }

    public class MyFile
    {
        public MyFile(string FileName, string Path, string Extension, int Position)
        {
            this.FileName = FileName;
            this.Path = Path;
            this.Extension = Extension;
            this.Position = Position;
        }

        public string FileName { get; set; }

        public string Path { get; set; }

        public string Extension { get; set; }

        public int Position { get; set; }
    }
}
