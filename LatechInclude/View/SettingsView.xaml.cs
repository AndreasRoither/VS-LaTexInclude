﻿
using LatechInclude.HelperClasses;
using System;
using System.Windows.Controls;

namespace LatechInclude.View
{
    /// <summary>
    /// Interaktionslogik für SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        ExplorerContextMenu ecm = new ExplorerContextMenu();

        /// <summary>
        /// SettingsView Contructor
        /// </summary>
        public SettingsView()
        {
            InitializeComponent();

            SaveWhiteList_ToggleB.IsChecked = Properties.Settings.Default.Setting_General_SaveWhiteList;
            StatusBar_ToggleB.IsChecked = Properties.Settings.Default.Setting_General_StatusBar;
            ContextStart_ToggleB.IsChecked = Properties.Settings.Default.Setting_General_ContextStartup;
        }

        /// <summary>
        /// SaveWhiteList Setting changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsCheckedChanged_SaveWhiteList(object sender, EventArgs e)
        {
            Properties.Settings.Default.Setting_General_SaveWhiteList = !Properties.Settings.Default.Setting_General_SaveWhiteList;
        }

        /// <summary>
        /// StatusBar Setting changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsCheckedChanged_StatusBar(object sender, EventArgs e)
        {
            Properties.Settings.Default.Setting_General_StatusBar = !Properties.Settings.Default.Setting_General_StatusBar;
        }

        /// <summary>
        /// ContextStart Setting changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsCheckedChanged_ContextStart(object sender, EventArgs e)
        {
            Properties.Settings.Default.Setting_General_ContextStartup = !Properties.Settings.Default.Setting_General_ContextStartup;
        }
    }
}
