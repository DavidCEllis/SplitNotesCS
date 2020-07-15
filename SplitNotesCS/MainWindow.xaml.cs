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
        private Parsing.NoteManager Notes = null;
        private Parsing.Templater Renderer;
        
        private readonly Properties.Settings Settings = Properties.Settings.Default;
        
        private string CurrentNoteFile = null;

        public MainWindow()
        {
            this.InitializeComponent();
            this.Renderer = new Parsing.Templater(this.Settings);
            this.RenderNotes(0);
        }

        // Connecting buttons
        private void OpenNotes_Click(object sender, RoutedEventArgs e)
        {
            this.OpenNotes();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWin = new SettingsWindow();
            settingsWin.ShowDialog();

            // Settings have been changed - refresh notes and connection
            // Notes and Renderer might update if it works correctly by reference
            if (this.CurrentNoteFile != null)
            {
                this.Notes = new Parsing.NoteManager(this.CurrentNoteFile, this.Settings);
            }

            this.Renderer = new Parsing.Templater(this.Settings);

        }

        private void OpenNotes()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|Markdown files (*.md)|*.md|HTML files (*.html)|*.html"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                this.CurrentNoteFile = openFileDialog.FileName;
                this.Notes = new Parsing.NoteManager(openFileDialog.FileName, this.Settings);
                this.RenderNotes(0);
            }
        }

        public void RenderNotes(int centreIndex)
        {
            string htmlData;
            if (this.Notes != null)
            {
                
                string rawNotes = this.Notes.GetRawNotes(centreIndex);
                htmlData = this.Renderer.RenderTemplate(rawNotes);

            }
            else
            {
                string defaultMessage = "Notes currently not loaded. Choose \"Open Notes\" from the file menu.";
                htmlData = this.Renderer.RenderTemplate(defaultMessage);
            }

            this.splitText.NavigateToString(htmlData);
        }

    }
}
