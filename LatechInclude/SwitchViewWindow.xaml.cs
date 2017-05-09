using LaTexInclude.ViewModel;
using MahApps.Metro.Controls;

namespace LaTexInclude
{
    /// <summary>
    /// Interaction logic for SwitchViewWindow.xaml
    /// </summary>
    public partial class SwitchViewWindow : MetroWindow
    {
        public SwitchViewWindow()
        {
            InitializeComponent();

            Closing += (s, e) => ViewModelLocator.Cleanup();
        }
    }
}