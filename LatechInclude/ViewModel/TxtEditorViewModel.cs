using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
    public class TxtEditorViewModel : ViewModelBase
    {
        public ICommand Fly { get; private set; }

        private static bool isFlyoutOpen;
        private static string _NotifyMessage = "";
        private static string _outputString = "";

        public TxtEditorViewModel()
        {
            Fly = new RelayCommand(Flyout);
            isFlyoutOpen = false;
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
            get { return _NotifyMessage; }
            set
            {
                _NotifyMessage = value;
                RaisePropertyChanged("NotifyMessage");
            }
        }

        /// <summary>
        /// outputString get and setter
        /// </summary>
        public string outputString
        {
            get { return _outputString; }
            set { _outputString = value; }
        }

        /// <summary>
        /// Method to set the notifymessage and open the flyout
        /// </summary>
        public void Flyout()
        {
            NotifyMessage = "Saved in workdirectory as output.tex";
            FlyoutOpen = true;
        }

        /// <summary>
        /// Saves the outputString to output.tex in the workdirectory
        /// </summary>
        public void SaveFileMethod()
        {
            try
            {
                System.IO.File.WriteAllText((System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\output.tex"), _outputString);
                NotifyMessage = "Saved";
                FlyoutOpen = true;
            }
            catch (Exception ex)
            {
                string outputString;
                outputString = Environment.NewLine + "Exception caught" + Environment.NewLine + "Date: " + DateTime.UtcNow.Date.ToString("dd/MM/yyyy") + ", Time: " + DateTime.Now.ToString("HH:mm:ss tt") + Environment.NewLine + ex.Message + Environment.NewLine + ex.ToString() + Environment.NewLine + "Runtime terminating: ";

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
