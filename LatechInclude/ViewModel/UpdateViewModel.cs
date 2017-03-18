using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LaTexInclude.Model;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.ComponentModel;
using System.Net;
using System.Windows.Input;

namespace LaTexInclude.ViewModel
{
    internal class UpdateViewModel : ViewModelBase
    {
        public ICommand DownloadCommand { get; private set; }
        public ICommand GithubCommand { get; private set; }

        private GetResponse response = null;

        private string path;

        public UpdateViewModel(GetResponse obj)
        {
            response = obj;

            this.Version = obj.Tag_name;
            this.Name = obj.Name;
            this.Body = obj.Body;

            this.DownloadCommand = new RelayCommand(DownloadMethod);
            this.GithubCommand = new RelayCommand(GithubMethod);

            ShowProgressBar = false;
            Progress = 0;
            Text = "";
        }

        private string version;

        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string body;

        public string Body
        {
            get { return body; }
            set { body = value; }
        }

        private int progress;

        public int Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                RaisePropertyChanged("Progress");
            }
        }

        private bool showBar;

        public bool ShowProgressBar
        {
            get { return showBar; }
            set
            {
                showBar = value;
                RaisePropertyChanged("ShowProgressBar");
            }
        }

        private string text;

        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                RaisePropertyChanged("Text");
            }
        }

        public void DownloadMethod()
        {
            ShowProgressBar = true;
            Text = "Downloading..";

            using (WebClient wc = new WebClient())
            {
                try
                {
                    using (CommonOpenFileDialog dlg = new CommonOpenFileDialog())
                    {
                        dlg.IsFolderPicker = true;
                        dlg.Title = "Choose a download path";
                        dlg.AddToMostRecentlyUsedList = false;
                        dlg.AllowNonFileSystemItems = false;
                        dlg.DefaultDirectory = Environment.CurrentDirectory;
                        dlg.EnsureFileExists = true;
                        dlg.EnsurePathExists = true;
                        dlg.EnsureReadOnly = false;
                        dlg.EnsureValidNames = true;
                        dlg.Multiselect = false;
                        dlg.ShowPlacesList = true;

                        if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
                        {
                            WebClient webClient = new WebClient();
                            webClient.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0)");
                            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                            webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                            webClient.DownloadFileAsync(new Uri(response.DownloadUrl), dlg.FileName + @"\LaTexInclude.exe");
                        }
                        else
                        {
                            Text = "Canceled";
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public void GithubMethod()
        {
            try
            {
                System.Diagnostics.Process.Start("https://github.com/AndiRoither/VS-LaTexInclude/releases");
            }
            catch
            {
                // welp sth happened
            }
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
        }

        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            Progress = 100;
            ShowProgressBar = false;
            Text = "Download finished.";
        }
    }
}