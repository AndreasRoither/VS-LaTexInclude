using LaTexInclude.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace LaTexInclude.View
{
    /// <summary>
    /// Interaktionslogik für UpdateView.xaml
    /// </summary>
    public partial class UpdateView : UserControl
    {
        public UpdateView()
        {
            InitializeComponent();
        }

        private void Later(object sender, RoutedEventArgs e)
        {
            UpdateViewModel uvm = new UpdateViewModel(true);
            UpdateViewModel.LaterClicked = true;
            var window = Window.GetWindow(this);
            window.Close();
        }
    }
}