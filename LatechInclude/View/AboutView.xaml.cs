using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace LaTexInclude.View
{
    /// <summary>
    /// Interaktionslogik für AboutView.xaml
    /// </summary>
    public partial class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
        }

        private void HyperlinkClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(((Hyperlink)sender).NavigateUri.ToString());
            //browser.Navigate(((Hyperlink)sender).NavigateUri);
        }
    }
}