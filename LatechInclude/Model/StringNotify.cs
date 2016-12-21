
using System.ComponentModel;

namespace LatechInclude.HelperClasses
{
    public class StringNotify : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _text;

        private void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
        }

        public StringNotify(string s)
        {
            this._text = s;
        }

        public StringNotify()
        {
        }

        public string text {
            get { return _text; }
            set
            {
                if (value != text)
                {
                    this._text = value;
                    NotifyPropertyChanged("fPath");
                }
            }
        }
    }
}

