using LaTexInclude.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace LaTexInclude.View
{
    /// <summary>
    /// Interaction logic for TxtEditorView.xaml
    /// </summary>
    public partial class TxtEditorView : UserControl
    {
        private TxtEditorViewModel tevm;

        /// <summary>
        /// TextEditorView Contructor
        /// </summary>
        public TxtEditorView()
        {
            InitializeComponent();
            tevm = new TxtEditorViewModel();
            this.DataContext = tevm;
        }

        /// <summary>
        /// When the RichTextBox has loaded, text will be set to output text
        /// </summary>
        private void RichTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            richTextBox.Document.Blocks.Clear();
            richTextBox.Document.Blocks.Add(new Paragraph(new Run(tevm.outputString)));
        }

        /// <summary>
        /// Save ButtonClick, saves the text to output.tex
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            tevm.outputString = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
            if (tevm.outputString != null && tevm.outputString != "")
            {
                tevm.SaveFileMethod();
                tevm.NotifyMessage = "Saved as output.tex in working directory";
            }
            else
                tevm.NotifyMessage = "Nothing in the textbox";
        }

        /// <summary>
        /// Replaces strings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnReplaceClick(object sender, RoutedEventArgs e)
        {
            if (searchTxtBox.Text.Length != 0)
            {
                string temp = tevm.outputString.Replace(searchTxtBox.Text.ToString(), replaceTxtBox.Text.ToString());
                tevm.outputString = temp;
                richTextBox.Document.Blocks.Clear();
                richTextBox.Document.Blocks.Add(new Paragraph(new Run(tevm.outputString)));
                temp = null;
                tevm.NotifyMessage = "Replaced";
            }
            else
                tevm.NotifyMessage = "Search textbox is empty";
            tevm.FlyoutOpen = true;
        }

        private void CopyToClipboard(object sender, RoutedEventArgs e)
        {
            string outputString = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
            if (outputString != null && outputString != "")
            {
                outputString = outputString.Replace("\r\n", "\r");
                System.Windows.Forms.Clipboard.SetText(outputString);
                tevm.NotifyMessage = "Copied to clipboard";
            }
            else
                tevm.NotifyMessage = "Nothing to copy";
        }
    }
}