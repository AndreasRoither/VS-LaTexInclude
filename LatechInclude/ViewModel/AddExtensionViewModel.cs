using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LatechInclude.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LatechInclude.ViewModel
{
    class AddExtensionViewModel : ViewModelBase
    {
        public ICommand AddExtensionCommand { get; private set; }
        public RelayCommand<Window> CloseWindowCommand { get; private set; }

        private List<string> _Languages = null;

        private static string _currentLanguage;
        private static string _maskedTxtBoxInput;

        MainViewModel mvm = null;

        public AddExtensionViewModel()
        {
            AddExtensionCommand = new RelayCommand(AddExtensionMethod);
            this.CloseWindowCommand = new RelayCommand<Window>(this.CloseWindow);

            _Languages = new List<string>();
            _maskedTxtBoxInput = "";

            mvm = new MainViewModel();

            _Languages = mvm.Languages;
        }

        public override void Cleanup()
        {
            AddExtensionCommand = null;
            CloseWindowCommand = null;

            base.Cleanup();
        }

        public List<string> Languages
        {
            get { return _Languages; }
            set { _Languages = value; }
        }

        public string maskedTxtBoxInput
        {
            get { return _maskedTxtBoxInput; }
            set { _maskedTxtBoxInput = value; }
        }

        public void AddExtensionMethod()
        {
            if (_maskedTxtBoxInput != "" && _maskedTxtBoxInput != ".")
            {
                List<WhiteList> tempList = new List<WhiteList>();
                tempList = mvm.whiteList;


                tempList.Add(new WhiteList
                {
                    Language = CurrentLanguage,
                    Extension = _maskedTxtBoxInput
                });

                //tempList.Sort(delegate (WhiteList w1, WhiteList w2) { return w1.Extension.CompareTo(w2.Extension); });
                tempList.Sort();
                mvm.whiteList = tempList;
            }
        }

        public string CurrentLanguage
        {
            get { return _currentLanguage; }
            set { _currentLanguage = value; }
        }

        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }
    }
}
