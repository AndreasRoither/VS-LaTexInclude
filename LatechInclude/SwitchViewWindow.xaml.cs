using LatechInclude.ViewModel;
using System.Windows;

namespace LatechInclude
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
