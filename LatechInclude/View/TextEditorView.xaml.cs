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
    /// Interaktionslogik für TextEditorView.xaml
    /// </summary>
    public partial class TextEditorView : UserControl
    {
        TextEditorViewModel tevm = new TextEditorViewModel();

        public TextEditorView()
        {
            InitializeComponent();
        }

        private void RichTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            richTextBox.Document.Blocks.Clear();
            richTextBox.Document.Blocks.Add(new Paragraph(new Run(tevm.outputString)));
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            tevm.outputString = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd).Text;
            tevm.SaveFileMethod();
            tevm.IsFlyoutOpen = true;
            tevm.NotifyMessage = "blalb";
        }
    }
}
