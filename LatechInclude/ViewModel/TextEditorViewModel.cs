using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LatechInclude.ViewModel
{
    class TextEditorViewModel : ViewModelBase
    {
        public static string _outputString = null;
        public ICommand SaveFileCommand { get; private set; }

        public TextEditorViewModel(string outputString)
        {
            _outputString = outputString;
            Init();
        }

        public TextEditorViewModel()
        {
            Init();
        }

        public void Init()
        {
            SaveFileCommand = new RelayCommand(SaveFileMethod);
        }

        public string outputString
        {
            get { return _outputString; }
            set { _outputString = value; }
        }
          
        public void SaveFileMethod()
        {
            try
            {
                System.IO.File.WriteAllText((System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\output.tex"), _outputString);
            }
            catch(Exception ex)
            {
                string output = ex.ToString();
                System.IO.File.WriteAllText((System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\CrashLog.txt"), output);
                outputString = null;
            }
            
        }
    }
}
