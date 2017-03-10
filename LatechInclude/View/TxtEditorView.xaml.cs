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
            tevm.SaveFileMethod();
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
            }
        }
    }
}