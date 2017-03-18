using System.Collections.Generic;

namespace LaTexInclude.Model
{
    public class GetResponse
    {
        public static string downloadUrl;

        public string DownloadUrl
        {
            get { return downloadUrl; }
            set { downloadUrl = value; }
        }

        private string url;

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        private string id;

        public string ID
        {
            get { return id; }
            set { id = value; }
        }

        private string tag;

        public string Tag_name
        {
            get { return tag; }
            set { tag = value; }
        }

        private string createdat;

        public string Created_at
        {
            get { return createdat; }
            set { createdat = value; }
        }

        private string publishedat;

        public string Published_at
        {
            get { return publishedat; }
            set { publishedat = value; }
        }

        private string prerelease;

        public string Prerelease
        {
            get { return prerelease; }
            set { prerelease = value; }
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

        private string draft;

        public string Draft
        {
            get { return draft; }
            set { draft = value; }
        }

        private string html_url;

        public string Html_url
        {
            get { return html_url; }
            set { html_url = value; }
        }

        private List<asssetsarray> assets;

        public List<asssetsarray> Assets
        {
            get { return assets; }
            set { assets = value; }
        }
    }

    public class asssetsarray
    {
        private string browser_download_url;

        public string Browser_download_url
        {
            get { return GetResponse.downloadUrl; }
            set { GetResponse.downloadUrl = value; }
        }
    }
}