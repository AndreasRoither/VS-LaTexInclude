using System;
using System.ComponentModel;

namespace LaTexInclude.HelperClasses
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

        /// <summary>
        /// Gets or sets the language of the file
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the extension
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Compare a whitelist object with another
        /// </summary>
        /// <param name="that">the whitelist object to compare</param>
        /// <returns></returns>
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