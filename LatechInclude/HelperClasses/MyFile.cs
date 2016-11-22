using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatechInclude.HelperClasses
{
    public class MyFile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string Obj)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(Obj));
            }
        }

        public MyFile(string FileName, string Path, string Extension, int Position)
        {
            this.FileName = FileName;
            this.Path = Path;
            this.Extension = Extension;
            this.Position = Position;
        }
        public int Position { get; set; }

        public string FileName { get; set; }

        public string Extension { get; set; }

        public string Path { get; set; } 
    }
}
