using Microsoft.Win32;
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


namespace SplitNotesCS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NoteParser notes;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        // Connecting buttons
        private void OpenNotes_Click(object sender, RoutedEventArgs e)
        {
            this.GetNotes();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            
        }

        // Doing things
        private FlowDocument GetDefaultDocument()
        {
            var mainText = new Paragraph();
            var message = new Run("Attempting to Connect to Livesplit Server.")
            {
                FontSize = 40,
                FontWeight = FontWeights.Bold
            };

            mainText.Inlines.Add(message);

            var defaultDoc = new FlowDocument();
            defaultDoc.Blocks.Add(mainText);

            return defaultDoc;

        }

        private void GetNotes()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|Markdown files (*.md)|*.md|HTML files (*.html)|*.html"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                this.notes = new NoteParser(openFileDialog.FileName);
                this.RenderNotes(0, 1);
            }
        }

        private void RenderNotes(int startIndex, int endIndex, bool useMarkdown = false)
        {
            string noteText = this.notes.getNotes(startIndex, endIndex);
            FlowDocument activeNotes;
            // Convert from Markdown
            if (false) // useMarkdown
            {
                
            }
            else
            {
                activeNotes = new FlowDocument();
                var segmentText = new Paragraph(new Run(noteText));
                
                activeNotes.Blocks.Add(segmentText);
            }
            
        }

    }
}
