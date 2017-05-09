using MahApps.Metro.Controls;
using System.Windows.Controls;

namespace LaTexInclude.View
{
    /// <summary>
    /// Interaktionslogik für test.xaml
    /// </summary>
    public partial class test : UserControl
    {
        public test()
        {
            InitializeComponent();
        }

        private void HamburgerMenuControl_OnItemClick(object sender, ItemClickEventArgs e)
        {
            // set the content
            this.HamburgerMenuControl.Content = e.ClickedItem;
            // close the pane
            this.HamburgerMenuControl.IsPaneOpen = false;
        }
    }
}
