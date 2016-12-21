using LatechInclude.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LatechInclude.View
{
    /// <summary>
    /// Interaction logic for TxtEditorView.xaml
    /// </summary>
    public partial class TxtEditorView : UserControl
    {
        TxtEditorViewModel tevm;

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
    }
}
