
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using LatechInclude.HelperClasses;
using System.Windows.Controls;
using LatechInclude.ViewModel;

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
            ContextStart_ToggleB.IsChecked = Properties.Settings.Default.Setting_General_ContextStartup;
        }

        private void IsCheckedChanged_SaveWhiteList(object sender, EventArgs e)
        {
            Properties.Settings.Default.Setting_General_SaveWhiteList = !Properties.Settings.Default.Setting_General_SaveWhiteList;
        }

        private void IsCheckedChanged_StatusBar(object sender, EventArgs e)
        {
            Properties.Settings.Default.Setting_General_StatusBar = !Properties.Settings.Default.Setting_General_StatusBar;
        }

        private void IsCheckedChanged_ContextStart(object sender, EventArgs e)
        {
            Properties.Settings.Default.Setting_General_ContextStartup = !Properties.Settings.Default.Setting_General_ContextStartup;
        }
    }
}
