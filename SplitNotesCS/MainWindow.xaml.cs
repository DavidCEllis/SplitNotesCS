using CefSharp;
using CefSharp.Wpf;
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
        private int LastIndex = 0;

        private ChromiumWebBrowser Browser;

        private readonly Properties.Settings Settings = Properties.Settings.Default;
        
        private string CurrentNoteFile = null;

        public MainWindow()
        {
            this.InitializeComponent();

            // Register a window closing to save settings and 
            this.Loaded += this.MainWindow_Loaded;
            this.Closed += this.MainWindow_Closed;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Prepare note renderer
            this.Renderer = new Parsing.Templater(this.Settings);

            // Add chrome component as child of border
            this.Browser = new ChromiumWebBrowser();
            this.BrowserContainer.Child = this.Browser;
            this.Browser.MenuHandler = new Utilities.NullContextMenu();

            this.Browser.Loaded += this.RenderNotes;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Cef.Shutdown();
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
            this.RenderNotes(this.LastIndex);

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
                this.RenderNotes(this.LastIndex);
            }
        }

        public void RenderNotes(object sender, RoutedEventArgs e)
        {
            this.RenderNotes(this.LastIndex);
        }

        public void RenderNotes(int centreIndex)
        {
            this.LastIndex = centreIndex;
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

            this.Browser.LoadHtml(htmlData);
        }

    }
}
