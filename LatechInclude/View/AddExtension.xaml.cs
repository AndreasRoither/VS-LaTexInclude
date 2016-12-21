using LatechInclude.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LatechInclude.View
{
    /// <summary>
    /// Interaktionslogik für AddExtension.xaml
    /// </summary>
    public partial class AddExtension : UserControl
    {
        AddExtensionViewModel aevm = null;

        public AddExtension()
        {
            InitializeComponent();

            aevm = new AddExtensionViewModel();
            aevm.CurrentLanguage = comboBox.SelectedItem as string;
        }

        private void OnLanguageSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            aevm.CurrentLanguage = (sender as ComboBox).SelectedItem as string;
        }

        private void OnTxtBox_KeyUp(object sender, KeyEventArgs e)
        {
            string temp = maskedTextBox.Text.ToString();
            temp = new string(temp.Where(c => !char.IsWhiteSpace(c)).ToArray());
            aevm.maskedTxtBoxInput = temp;
        }
    }
}
