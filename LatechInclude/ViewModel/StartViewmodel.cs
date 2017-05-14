using GalaSoft.MvvmLight;
using System;
using System.ComponentModel;

namespace LaTexInclude.ViewModel
{
    internal class StartViewmodel : ViewModelBase, INotifyPropertyChanged
    {
        public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged
         = delegate { };

        private static void NotifyStaticPropertyChanged(string propertyName)
        {
            StaticPropertyChanged(null, new PropertyChangedEventArgs(propertyName));
        }

        private static int selectedIndex;

        public static int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                NotifyStaticPropertyChanged("SelectedIndex");
            }
        }
    }
}