using System.ComponentModel;

namespace LaTexInclude.HelperClasses
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

        /// <summary>
        /// MyFile Constructor
        /// </summary>
        /// <param name="FileName">Name of the file</param>
        /// <param name="Path">Path of the file</param>
        /// <param name="Extension">Extension of the file</param>
        /// <param name="Position">The order/position of the file</param>
        public MyFile(string FileName, string Path, string Extension, int Position)
        {
            this.FileName = FileName;
            this.Path = Path;
            this.Extension = Extension;
            this.Position = Position;
        }

        /// <summary>
        /// Gets or sets the filename
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the path of the file
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the extension of the file
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Gets or sets the position of the file in the list
        /// </summary>
        public int Position { get; set; }
    }
}