using Microsoft.Win32;
using SplitNotesCS.Networking;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

        private readonly Properties.Settings Settings = Properties.Settings.Default;

        private readonly int updateInterval = 100;  // Update 10x a second
        volatile private bool closeThread = false;
        private Thread networkThread;
        private LivesplitConnection LSConnection;

        private int SplitOffset = 0;  // This is an offset for manually moving through splits

        private string CurrentNoteFile = null;

        public MainWindow()
        {
            this.InitializeComponent();

            // Register a window closing to save settings and 
            this.Loaded += this.MainWindow_Loaded;
            this.Closing += this.MainWindow_Closing;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Height = this.Settings.windowHeight;
            this.Width = this.Settings.windowWidth;

            // Prepare note renderer
            this.Renderer = new Parsing.Templater(this.Settings);

            this.RenderNotes(0);
            this.SetStatus("Connecting to Livesplit Server.");

            // Start networking thread
            this.closeThread = false;
            this.networkThread = new Thread(new ThreadStart(this.ConnectLivesplit));
            this.networkThread.IsBackground = true;
            this.networkThread.Start();
        }
        private void MainWindow_Closing(object sender, EventArgs e)
        {
            // Store window width and height
            this.Settings.windowHeight = this.Height;
            this.Settings.windowWidth = this.Width;

            // Store current settings.
            this.Settings.Save();
            
            // Tell the thread to close
            this.closeThread = true;
            Thread.Sleep(this.updateInterval * 2); // Sleep to allow the thread to close
            
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
            if (this.CurrentNoteFile != null)
            {
                this.Notes = new Parsing.NoteManager(this.CurrentNoteFile, this.Settings);
            }

            this.Renderer = new Parsing.Templater(this.Settings);
            this.RenderNotes(this.LastIndex);

        }

        private void ToggleTopmost(object sender, RoutedEventArgs e)
        {
            this.Topmost = this.OnTopMenu.IsChecked;
        }

        // The main livesplit thread code
        private void ConnectLivesplit()
        {
            LSConnection = new LivesplitConnection(this.Settings.livesplitHostname, this.Settings.livesplitPort);

            while (!this.closeThread)
            {
                // If not connected try to connect
                if (this.LSConnection.Connected == false)
                {
                    try
                    {
                        this.LSConnection.Connect();
                        this.Dispatcher.Invoke(() => this.SetStatus("Connected to Livesplit."));
                    }
                    catch (Exception e)
                    {
                        this.Dispatcher.Invoke(() => this.SetStatus($"Failed to Connect to Livesplit: {e.Message}"));
                    }
                }
                // If connected, get the current split index
                else
                {
                    try
                    {
                        int livesplitIndex = this.LSConnection.GetIndex() + this.SplitOffset; // Get the current index and combine with the offset
                        if (this.LastIndex != livesplitIndex)
                        {
                            this.Dispatcher.Invoke(() => this.RenderNotes(livesplitIndex));
                        }
                    }
                    catch (Exception e)
                    {
                        this.Dispatcher.Invoke(() => this.SetStatus($"Lost Connection to Livesplit - retrying: {e.Message}"));
                    }
                }
                Thread.Sleep(this.updateInterval);   
            }

            this.LSConnection.Disconnect();
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

            this.Browser.NavigateToString(htmlData);
        }

        // Update the statusbar message
        public void SetStatus(string message)
        {
            this.StatusText.Text = message;
        }

    }
}
