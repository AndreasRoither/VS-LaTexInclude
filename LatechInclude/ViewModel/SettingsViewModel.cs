using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
