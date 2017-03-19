using System.Collections.Generic;

namespace LaTexInclude.Model
{
    /// <summary>
    /// The class for the HTTPWebRequest response
    /// </summary>
    public class GetResponse
    {
        public static string downloadUrl;
        private string url;
        private string id;
        private string tag;
        private string createdat;
        private string publishedat;
        private bool prerelease;
        private string name;
        private string body;
        private bool draft;
        private string html_url;
        private List<asssetsarray> assets;

        /// <summary>
        /// Gets or sets latest Release exe download url
        /// </summary>
        public string DownloadUrl
        {
            get { return downloadUrl; }
            set { downloadUrl = value; }
        }

        /// <summary>
        /// Gets or sets release url without tags
        /// </summary>
        public string Url
        {
            get { return this.url; }
            set { url = value; }
        }

        /// <summary>
        /// Gets or sets github id
        /// </summary>
        public string ID
        {
            get { return this.id; }
            set { id = value; }
        }

        /// <summary>
        /// Gets or sets tag name of the release
        /// </summary>
        public string Tag_name
        {
            get { return this.tag; }
            set { tag = value; }
        }

        /// <summary>
        /// Gets or sets created date
        /// </summary>
        public string Created_at
        {
            get { return this.createdat; }
            set { createdat = value; }
        }

        /// <summary>
        /// Gets or sets published date
        /// </summary>
        public string Published_at
        {
            get { return this.publishedat; }
            set { publishedat = value; }
        }

        /// <summary>
        /// Gets or sets prerelase
        /// </summary>
        public bool Prerelease
        {
            get { return this.prerelease; }
            set { prerelease = value; }
        }

        /// <summary>
        /// Gets or sets the name of the relese
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the description of the release
        /// </summary>
        public string Body
        {
            get { return this.body; }
            set { body = value; }
        }

        /// <summary>
        /// Gets or sets draft
        /// </summary>
        public bool Draft
        {
            get { return this.draft; }
            set { draft = value; }
        }

        /// <summary>
        /// Gets or sets release url with tag
        /// </summary>
        public string Html_url
        {
            get { return this.html_url; }
            set { html_url = value; }
        }

        /// <summary>
        /// Gets or sets assets
        /// </summary>
        public List<asssetsarray> Assets
        {
            get { return this.assets; }
            set { assets = value; }
        }
    }

    /// <summary>
    /// Helper class for Assets
    /// </summary>
    public class asssetsarray
    {
        private string browser_download_url;

        /// <summary>
        /// Gets or sets the browser download url of an asset
        /// </summary>
        public string Browser_download_url
        {
            get { return GetResponse.downloadUrl; }
            set { GetResponse.downloadUrl = value; }
        }
    }
}