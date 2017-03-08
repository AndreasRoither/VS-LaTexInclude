using LatechInclude.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        /// <summary>
        /// Sets the language in the ViewModel
        /// </summary>
        private void OnLanguageSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            aevm.CurrentLanguage = (sender as ComboBox).SelectedItem as string;
        }

        /// <summary>
        /// Saves changes to the ViewModel
        /// </summary>
        private void OnTxtBox_KeyUp(object sender, KeyEventArgs e)
        {  
            string temp = textBox.Text.ToString();

            temp = new string(temp.Where(c => !char.IsWhiteSpace(c)).ToArray());
            aevm.TxtBoxInput = temp; 
        }

        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Check for a naughty character in the KeyDown event.
            if (System.Text.RegularExpressions.Regex.IsMatch(e.Key.ToString().ToLower(), @"[^0-9^+^\-^\/^\*^\(^\)]"))
            {
                // Stop the character from being entered into the control since it is illegal.
                e.Handled = true;
            }
        }
    }
}
