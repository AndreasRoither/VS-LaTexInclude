
using System.ComponentModel;

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
        public string FileName { get; set; }

        public string Path { get; set; }

        public string Extension { get; set; }

        public int Position { get; set; } 
    }
}
