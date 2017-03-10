using System.ComponentModel;

namespace LaTexInclude.HelperClasses
{
    public class Languages : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _Language;

        private void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
        }

        public Languages(string Language)
        {
            this._Language = Language;
        }

        public string Language
        {
            get
            {
                return _Language;
            }
            set
            {
                if (value != _Language)
                {
                    this._Language = value;
                    NotifyPropertyChanged("Language");
                }
            }
        }

        public string text
        {
            get { return text; }
            set
            {
                if (value != text)
                {
                    text = value;
                    NotifyPropertyChanged("Text");
                }
            }
        }
    }
}