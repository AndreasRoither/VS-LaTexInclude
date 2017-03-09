
using System;
using System.ComponentModel;

namespace LatechInclude.HelperClasses
{
    public class WhiteList : INotifyPropertyChanged, IComparable<WhiteList>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
        }

        /// <summary>
        /// WhiteList constructor
        /// </summary>
        public WhiteList()
        {
        }

        /// <summary>
        /// WhiteList Constructor
        /// </summary>
        /// <param name="Language">The language used</param>
        /// <param name="Extension">The extension</param>
        public WhiteList(string Language, string Extension)
        {
            this.Language = Language;
            this.Extension = Extension;
        }

        public string Language { get; set; }

        public string Extension { get; set; }

        public int CompareTo(WhiteList that)
        {
            if (this.Language == that.Language)
            {
                return this.Extension.CompareTo(that.Extension);
            }
            else
            {
                return this.Language.CompareTo(that.Language);
            }
        }

    }
}
