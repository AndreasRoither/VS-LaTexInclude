
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LatechInclude.HelperClasses;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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
        public ICommand NameSortCommand { get; private set; }
        public ICommand ExtensionSortCommand { get; private set; }

        private CommonOpenFileDialog dlg = new CommonOpenFileDialog();
        private static TrulyObservableCollection<MyFile> _fileList = new TrulyObservableCollection<MyFile>();

        public MainViewModel()
        {
            PathFolderDialogCommand = new RelayCommand(PathFolderDialogMethod);
            TexMakerCommand = new RelayCommand(TexMakerMethod);
            NameSortCommand = new RelayCommand(NameSortMethod);
            ExtensionSortCommand = new RelayCommand(ExtensionSortMethod);

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
                string[] files = Directory.GetFiles(dlg.FileName, "*", SearchOption.AllDirectories);
                _fileList.Clear();

                int i = 1;
                foreach (string file in files)
                {
                    _fileList.Add(new MyFile(System.IO.Path.GetFileNameWithoutExtension(file), file, System.IO.Path.GetExtension(file), i));
                    i++;
                }
            }
        }

        public void TexMakerMethod()
        {
            if (_fileList.Count == 0)
            {
                
            }
        }

        public void NameSortMethod()
        {
            _fileList = new TrulyObservableCollection<MyFile>(from file in _fileList orderby file.FileName select file);
            int i = 1;

            foreach (MyFile file in _fileList)
            {
                file.Position = i;
                i++;
            }
        }
        public void ExtensionSortMethod()
        {
            _fileList = new TrulyObservableCollection<MyFile>(from file in _fileList orderby file.Extension select file);
            int i = 1;
            foreach (MyFile file in _fileList)
            {
                file.Position = i;
                i++;
            }
        }  
    }
}