using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LatechInclude.ViewModel
{
    class TextEditorViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public static string _outputString = null;
        private static bool _isOpen = false;
        private static string _notifyMessage = "Saved to workdirectory as output.tex";

        public event PropertyChangedEventHandler PropertyChanged;

        public TextEditorViewModel(string outputString)
        {
            _outputString = outputString;
        }

        public TextEditorViewModel()
        {
        }

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                Console.WriteLine(propertyName);
                var e = new PropertyChangedEventArgs(propertyName);
                this.PropertyChanged(this, e);
            }
        }

        public string outputString
        {
            get { return _outputString; }
            set { _outputString = value; }
        }

        public bool IsFlyoutOpen
        {
            get { return _isOpen; }
            set
            {
                if (_isOpen == value) return;
                _isOpen = value;
                OnPropertyChanged("IsFlyoutOpen");
            }
        }

        public string NotifyMessage
        {
            get { return _notifyMessage; }
            set
            {
                if (_notifyMessage == value) return;
                _notifyMessage = value;
                OnPropertyChanged("NotifyMessage");
            }
        }

        public void SaveFileMethod()
        {
            try
            {
                System.IO.File.WriteAllText((System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\output.tex"), _outputString);
            }
            catch(Exception ex)
            {
                string output = ex.ToString();
                System.IO.File.WriteAllText((System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\CrashLog.txt"), output);
                outputString = null;
            }       
        }
    }
}
