namespace LaTexInclude.HelperClasses
{
    internal class TexTemplate
    {
        public TexTemplate(string Type, string Path)
        {
            this.Type = Type;
            this.Path = Path;
        }

        public string Path { get; set; }

        public string Type { get; set; }
    }
}