using LaTexInclude.ViewModel;
using System.Windows;

namespace LaTexInclude
{
    /// <summary>
    /// Interaction logic for SwitchViewWindow.xaml
    /// </summary>
    public partial class SwitchViewWindow : Window
    {
        public SwitchViewWindow()
        {
            InitializeComponent();

            Closing += (s, e) => ViewModelLocator.Cleanup();
        }
    }
}