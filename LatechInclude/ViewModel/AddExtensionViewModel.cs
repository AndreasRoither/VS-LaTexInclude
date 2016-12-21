using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LatechInclude.HelperClasses;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace LatechInclude.ViewModel
{
    /// <summary>
    /// This class contains properties that the SwitchView can bind to
    /// <para>
    /// Inherits from ViewModelBase
    /// </para>
    /// </summary>
    class AddExtensionViewModel : ViewModelBase
    {
        public ICommand AddExtensionCommand { get; private set; }
        public RelayCommand<Window> CloseWindowCommand { get; private set; }

        private List<string> _Languages = null;
        private bool isFlyoutOpen;
        private string _NotifyMessage = "";

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
            _Languages.Remove("All");

            isFlyoutOpen = false;
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

        public bool FlyoutOpen
        {
            get { return isFlyoutOpen; }
            set
            {
                isFlyoutOpen = value;
                RaisePropertyChanged("FlyoutOpen");
            }
        }

        public string NotifyMessage
        {
            get { return _NotifyMessage; }
            set
            {
                _NotifyMessage = value;
                RaisePropertyChanged("NotifyMessage");
            }
        }

        public void AddExtensionMethod()
        {
            if (_maskedTxtBoxInput != "" && _maskedTxtBoxInput != ".")
            {       
                List<WhiteList> tempList = new List<WhiteList>();
                tempList = mvm.whiteList;
                Boolean notFound = true;

                foreach (WhiteList wl in tempList)
                {
                    if (wl.Extension == _maskedTxtBoxInput) notFound = false;
                }

                if (notFound)
                {
                    tempList.Add(new WhiteList
                    {
                        Language = CurrentLanguage,
                        Extension = _maskedTxtBoxInput
                    });

                    tempList.Sort();
                    mvm.whiteList = tempList;

                    NotifyMessage = "Extension added";
                    FlyoutOpen = true;
                }
                else
                {
                    NotifyMessage = "Already in the WhiteList";
                    FlyoutOpen = true;
                }
            }
            else
            {
                NotifyMessage = "Textbox is empty, add something";
                FlyoutOpen = true;
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
