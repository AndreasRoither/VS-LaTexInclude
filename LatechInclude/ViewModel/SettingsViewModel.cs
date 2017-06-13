using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LaTexInclude.HelperClasses;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Reflection;
using System.Windows.Input;

namespace LaTexInclude.ViewModel
{
    /// <summary>
    /// This class contains properties that the SwitchView can data bind to.
    /// <para>
    /// Inherits from ViewModelBase
    /// </para>
    /// </summary>
    public class SettingsViewModel : ViewModelBase
    {
        private bool isFlyoutOpen;
        private string notifyMessage = string.Empty;
        private static string customPath;

        private ExplorerContextMenu ecm = new ExplorerContextMenu();

        public static ICommand FileCommand { get; private set; }
        public static ICommand FolderCommand { get; private set; }
        public static ICommand PathDialogCommand { get; private set; }
        public static ICommand ClearPathCommand { get; private set; }

        /// <summary>
        /// SettingsViewModel constructor
        /// </summary>
        public SettingsViewModel()
        {
            FileCommand = new RelayCommand(FileMethod);
            FolderCommand = new RelayCommand(FolderMethod);
            PathDialogCommand = new RelayCommand(PathDialog);
            ClearPathCommand = new RelayCommand(ClearPathMethod);

            customPath = Properties.Settings.Default.Setting_General_CustomPath;
        }

        /// <summary>
        /// Flyout get and setter
        /// </summary>
        public bool FlyoutOpen
        {
            get { return isFlyoutOpen; }
            set
            {
                isFlyoutOpen = value;
                RaisePropertyChanged("FlyoutOpen");
            }
        }

        /// <summary>
        /// Notify message get and setter
        /// </summary>
        public string NotifyMessage
        {
            get { return notifyMessage; }
            set
            {
                if (notifyMessage == value) return;
                notifyMessage = value;
                RaisePropertyChanged("NotifyMessage");
            }
        }

        /// <summary>
        /// Gets or sets CustomPath
        /// </summary>
        public string CustomPath
        {
            get { return customPath; }
            set
            {
                if (customPath == value) return;
                customPath = value;
                RaisePropertyChanged("CustomPath");
            }
        }

        /// <summary>
        /// Adds a registry entry for files for LaTexInclude
        /// </summary>
        public void FileMethod()
        {
            Properties.Settings.Default.Setting_Advanced_FileRegistry = !Properties.Settings.Default.Setting_Advanced_FileRegistry;
            Properties.Settings.Default.Save();

            try
            {
                if (Properties.Settings.Default.Setting_Advanced_FileRegistry)
                {
                    string path_temp = System.Reflection.Assembly.GetExecutingAssembly().Location;

                    // sample usage to register
                    // get full path to self, %L is a placeholder for the selected file
                    string menuCommand = string.Format("\"{0}\" \"%L\"", path_temp);
                    ecm.Register_File("*", "LatexInclude", "Open with LaTexInclude", menuCommand);

                    NotifyMessage = "Added file context to the registry";
                }
                else
                {
                    ecm.Unregister_File("*", "LatexInclude");

                    NotifyMessage = "Removed file context from the registry";
                }

                FlyoutOpen = true;
            }
            catch (UnauthorizedAccessException)
            {
                Properties.Settings.Default.Setting_Advanced_FileRegistry = !Properties.Settings.Default.Setting_Advanced_FileRegistry;
                Properties.Settings.Default.Save();

                NotifyMessage = "Failed to acess the registry, restart as ADMIN";
                FlyoutOpen = true;
            }
            catch (Exception ex)
            {
                Properties.Settings.Default.Setting_Advanced_FileRegistry = !Properties.Settings.Default.Setting_Advanced_FileRegistry;
                Properties.Settings.Default.Save();

                string outputString;
                outputString = Environment.NewLine + "Exception caught" + Environment.NewLine + "Date: " + DateTime.UtcNow.Date.ToString("dd/MM/yyyy") + ", Time: " + DateTime.Now.ToString("HH:mm:ss tt") + Environment.NewLine + ex.Message + Environment.NewLine + ex.ToString() + Environment.NewLine;

                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\CrashLog.txt", true))
                {
                    file.WriteLine(outputString);
                }

                outputString = null;
            }
        }

        /// <summary>
        /// Adds a registry entry for folders for LaTexInclude
        /// </summary>
        public void FolderMethod()
        {
            Properties.Settings.Default.Setting_Advanced_FolderRegistry = !Properties.Settings.Default.Setting_Advanced_FolderRegistry;
            Properties.Settings.Default.Save();

            try
            {
                if (Properties.Settings.Default.Setting_Advanced_FolderRegistry)
                {
                    string path_temp = System.Reflection.Assembly.GetExecutingAssembly().Location;

                    // get full path to self, %L is a placeholder for the selected file
                    string menuCommand = string.Format("\"{0}\" \"%L\"", path_temp);

                    ecm.Register_Folder("LatexInclude", "Open with LatexInclude", path_temp, menuCommand);

                    NotifyMessage = "Added folder context to the registry";
                }
                else
                {
                    ecm.Unregister_Folder("LatexInclude");
                    NotifyMessage = "Removed folder context from the registry";
                }

                FlyoutOpen = true;
            }
            catch (System.Security.SecurityException)
            {
                Properties.Settings.Default.Setting_Advanced_FolderRegistry = !Properties.Settings.Default.Setting_Advanced_FolderRegistry;
                Properties.Settings.Default.Save();

                NotifyMessage = "Failed to acess the registry, restart as ADMIN";
                FlyoutOpen = true;
            }
            catch (UnauthorizedAccessException)
            {
                Properties.Settings.Default.Setting_Advanced_FolderRegistry = !Properties.Settings.Default.Setting_Advanced_FolderRegistry;
                Properties.Settings.Default.Save();

                NotifyMessage = "Failed to acess the registry, restart as ADMIN";
                FlyoutOpen = true;
            }
            catch (Exception ex)
            {
                Properties.Settings.Default.Setting_Advanced_FolderRegistry = !Properties.Settings.Default.Setting_Advanced_FolderRegistry;
                Properties.Settings.Default.Save();

                string outputString;
                outputString = Environment.NewLine + "Exception caught" + Environment.NewLine + "Date: " + DateTime.UtcNow.Date.ToString("dd/MM/yyyy") + ", Time: " + DateTime.Now.ToString("HH:mm:ss tt") + Environment.NewLine + ex.Message + Environment.NewLine + ex.ToString() + Environment.NewLine;

                using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\CrashLog.txt", true))
                {
                    file.WriteLine(outputString);
                }

                outputString = null;
            }
        }

        /// <summary>
        /// Opens a path folder dialog
        /// </summary>
        public void PathDialog()
        {
            using (CommonOpenFileDialog dlg = new CommonOpenFileDialog())
            {
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

                if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    CustomPath = dlg.FileName;
                    Properties.Settings.Default.Setting_General_CustomPath = CustomPath;
                    NotifyMessage = "Path saved";
                    FlyoutOpen = true;
                }
            }
        }

        /// <summary>
        /// Clear path textbox and setting
        /// </summary>
        public void ClearPathMethod()
        {
            if (CustomPath != String.Empty)
            {
                CustomPath = "";
                Properties.Settings.Default.Setting_General_CustomPath = "";
                NotifyMessage = "Path reset";
                FlyoutOpen = true;
            }
            else
            {
                NotifyMessage = "It's already empty.";
                FlyoutOpen = true;
            }
        }
    }
}