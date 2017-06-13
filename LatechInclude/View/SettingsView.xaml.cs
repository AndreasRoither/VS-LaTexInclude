using LaTexInclude.ViewModel;
using System;
using System.Windows.Controls;

namespace LaTexInclude.View
{
    /// <summary>
    /// Interaktionslogik für SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        private SettingsViewModel svm;

        /// <summary>
        /// SettingsView Contructor
        /// </summary>
        public SettingsView()
        {
            InitializeComponent();

            svm = new SettingsViewModel();

            SaveWhiteList_Toggle.IsChecked = Properties.Settings.Default.Setting_General_SaveWhiteList;
            StatusBar_Toggle.IsChecked = Properties.Settings.Default.Setting_General_StatusBar;
            ContextStart_Toggle.IsChecked = Properties.Settings.Default.Setting_General_ContextStartup;
            RelativePath_Toggle.IsChecked = Properties.Settings.Default.Setting_General_UseRelativePath;
            CustomPath_Toggle.IsChecked = Properties.Settings.Default.Setting_General_UseCustomPath;
            customPath.IsEnabled = Properties.Settings.Default.Setting_General_UseCustomPath;
            customPath.Text = Properties.Settings.Default.Setting_General_CustomPath;
            CopyToClipBoard_Toggle.IsChecked = Properties.Settings.Default.Setting_General_CopyToClipboard;
            this.DataContext = svm;
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
            MainViewModel mvm = new MainViewModel();
            mvm.StatusText = "";
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

        /// <summary>
        /// RelativePath Setting changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsCheckedChanged_RelativePath(object sender, EventArgs e)
        {
            Properties.Settings.Default.Setting_General_UseRelativePath = !Properties.Settings.Default.Setting_General_UseRelativePath;
        }

        /// <summary>
        /// CustomPath Setting changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IsCheckedChanged_CustomPath(object sender, EventArgs e)
        {
            Properties.Settings.Default.Setting_General_UseCustomPath = !Properties.Settings.Default.Setting_General_UseCustomPath;
            customPath.IsEnabled = Properties.Settings.Default.Setting_General_UseCustomPath;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            Properties.Settings.Default.Setting_General_CustomPath = customPath.Text;
        }

        private void IsCheckedChanged_CopyToClipboard(object sender, EventArgs e)
        {
            Properties.Settings.Default.Setting_General_CopyToClipboard = !Properties.Settings.Default.Setting_General_CopyToClipboard;
        }
    }
}