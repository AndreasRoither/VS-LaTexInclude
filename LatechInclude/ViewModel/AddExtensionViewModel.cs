using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LaTexInclude.HelperClasses;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace LaTexInclude.ViewModel
{
    /// <summary>
    /// This class contains properties that the SwitchView can bind to
    /// <para>
    /// Inherits from ViewModelBase
    /// </para>
    /// </summary>
    internal class AddExtensionViewModel : ViewModelBase
    {
        public ICommand AddExtensionCommand { get; private set; }
        public RelayCommand<Window> CloseWindowCommand { get; private set; }

        private List<string> _Languages = new List<string>();
        private bool isFlyoutOpen;
        private string _NotifyMessage = "";

        private static string _currentLanguage;
        private static string _TxtBoxInput;

        private MainViewModel mvm = null;

        /// <summary>
        /// Instantiates a new AddExtensionViewModel
        /// </summary>
        public AddExtensionViewModel()
        {
            AddExtensionCommand = new RelayCommand(AddExtensionMethod);
            this.CloseWindowCommand = new RelayCommand<Window>(this.CloseWindow);

            _Languages = new List<string>();
            _TxtBoxInput = "";

            mvm = new MainViewModel();

            foreach (string s in mvm.Languages)
            {
                if (s != "All") _Languages.Add(s);
            }

            isFlyoutOpen = false;
        }

        public override void Cleanup()
        {
            AddExtensionCommand = null;
            CloseWindowCommand = null;
            mvm = null;

            base.Cleanup();
        }

        public string CurrentLanguage
        {
            get { return _currentLanguage; }
            set { _currentLanguage = value; }
        }

        public List<string> Languages
        {
            get { return _Languages; }
            set { _Languages = value; }
        }

        public string TxtBoxInput
        {
            get { return _TxtBoxInput; }
            set { _TxtBoxInput = value; }
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

        /// <summary>
        /// Adds the extension to the whitelist
        /// </summary>
        public void AddExtensionMethod()
        {
            if (_TxtBoxInput != "" && _TxtBoxInput != ".")
            {
                List<WhiteList> tempList = new List<WhiteList>();
                string temp = "." + _TxtBoxInput;
                tempList = mvm.WhiteList;
                Boolean notFound = true;

                foreach (WhiteList wl in tempList)
                {
                    if (wl.Extension == temp) notFound = false;
                }

                if (notFound)
                {
                    tempList.Add(new WhiteList
                    {
                        Language = CurrentLanguage,
                        Extension = temp
                    });

                    tempList.Sort();
                    mvm.WhiteList = tempList;

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

        private void CloseWindow(Window window)
        {
            if (window != null)
            {
                window.Close();
            }
        }
    }
}