
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using LatechInclude.HelperClasses;
using System.Windows.Controls;

namespace LatechInclude.View
{
    /// <summary>
    /// Interaktionslogik für SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        ExplorerContextMenu ecm = new ExplorerContextMenu();

        public SettingsView()
        {
            InitializeComponent();

            SaveWhiteList_ToggleB.IsChecked = Properties.Settings.Default.Setting_General_SaveWhiteList;
            StatusBar_ToggleB.IsChecked = Properties.Settings.Default.Setting_General_StatusBar;
        }

        private void IsCheckedChanged_SaveWhiteList(object sender, EventArgs e)
        {
            Properties.Settings.Default.Setting_General_SaveWhiteList = !Properties.Settings.Default.Setting_General_SaveWhiteList;
        }

        private void IsCheckedChanged_StatusBar(object sender, EventArgs e)
        {
            Properties.Settings.Default.Setting_General_StatusBar = !Properties.Settings.Default.Setting_General_StatusBar;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string path_temp = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // sample usage to register
            // get full path to self, %L is a placeholder for the selected file
            string menuCommand = string.Format("\"{0}\" \"%L\"",
                                                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            ecm.Register("*", "LatexIncludeMenu",
                                        "Open with LaTexInclude", menuCommand);
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            // sample usage to unregister
            ecm.Unregister("*", "LatexIncludeMenu");
        }
    }
}
