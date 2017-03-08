using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LatechInclude.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LatechInclude.ViewModel
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
        private string _notifyMessage = "";
        ExplorerContextMenu ecm = new ExplorerContextMenu();

        public static ICommand FileCommand { get; private set; }
        public static ICommand FolderCommand { get; private set; }

        public SettingsViewModel()
        {
            FileCommand = new RelayCommand(FileMethod);
            FolderCommand = new RelayCommand(FolderMethod);
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
            get { return _notifyMessage; }
            set
            {
                if (_notifyMessage == value) return;
                _notifyMessage = value;
                RaisePropertyChanged("NotifyMessage");
            }
        }

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
            catch (UnauthorizedAccessException uae)
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
            catch(System.Security.SecurityException se)
            {
                Properties.Settings.Default.Setting_Advanced_FolderRegistry = !Properties.Settings.Default.Setting_Advanced_FolderRegistry;
                Properties.Settings.Default.Save();

                NotifyMessage = "Failed to acess the registry, restart as ADMIN";
                FlyoutOpen = true;
            }
            catch (UnauthorizedAccessException uae)
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
    }
}
