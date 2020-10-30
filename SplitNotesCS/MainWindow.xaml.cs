using Microsoft.Win32;
using SplitNotesCS.Networking;
using SplitNotesCS.Hotkeys;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        // Notemanager handles the text sanitizing and preparation of the individual splits
        // Templater takes the output from notemanager and gives the page to render
        // See the .cs source in the parsing folder.
        private Parsing.NoteManager Notes = null;
        private Parsing.Templater Renderer;

        private int LastIndex = 0;  // Used to check if it's necessary to update the HTML

        private readonly Properties.Settings Settings = Properties.Settings.Default;

        // Details for networking and the livesplit connection
        private readonly int updateInterval = 100;  // Update 10x a second - more than enough for this purpose
        volatile private bool closeThread = false;
        private Thread networkThread;
        private LivesplitConnection LSConnection;

        // Keyboard hook for the next/previous split function
        public HotkeyManager hotkeys;
        private int SplitOffset = 0;  // This is an offset for manually moving through splits

        private string CurrentNoteFile = null;  // Path to current set of splits

        public MainWindow()
        {
            this.InitializeComponent();

            // Register a window closing to save settings
            this.Loaded += this.MainWindow_Loaded;
            this.Closing += this.MainWindow_Closing;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Height = this.Settings.windowHeight;
            this.Width = this.Settings.windowWidth;
            this.Topmost = this.Settings.onTop;
            this.OnTopMenu.IsChecked = this.Settings.onTop;
            this.HotkeyToggle.IsChecked = this.Settings.hotkeysActive;

            // Prepare note renderer
            this.Renderer = new Parsing.Templater(this.Settings);

            this.RenderNotes(0);
            this.SetStatus("Connecting to Livesplit Server.");

            // Start networking thread
            this.closeThread = false;
            this.networkThread = new Thread(new ThreadStart(this.ConnectLivesplit));
            this.networkThread.IsBackground = true;
            this.networkThread.Start();

            this.hotkeys = new HotkeyManager();

            if (this.Settings.hotkeysActive)
            {
                this.EnableHotkeys();
            }
        }
        private void MainWindow_Closing(object sender, EventArgs e)
        {
            // Store window width and height
            this.Settings.windowHeight = this.Height;
            this.Settings.windowWidth = this.Width;
            this.Settings.onTop = this.Topmost;

            // Store current settings.
            this.Settings.Save();
            
            // Tell the thread to close
            this.closeThread = true;
            Thread.Sleep(this.updateInterval * 2); // Sleep to allow the thread to close
            
            // Cleanup Keyboard Hook
            if (this.Settings.hotkeysActive)
            {
                this.DisableHotkeys();
            }
            this.hotkeys.Cleanup();

        }

        // Connecting buttons
        private void OpenNotes_Click(object sender, RoutedEventArgs e)
        {
            this.OpenNotes();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var settingsWin = new SettingsWindow(this.hotkeys);

            // Disable hotkeys while the settings window is open
            this.DisableHotkeys();

            settingsWin.Topmost = this.Topmost;  // if this window is topmost, make settings topmost so it's not hidden.
            settingsWin.ShowDialog();

            // Enable hotkeys again
            if (this.Settings.hotkeysActive) { this.EnableHotkeys(); }

            // Settings have been changed - refresh notes and connection
            if (this.CurrentNoteFile != null)
            {
                this.Notes = new Parsing.NoteManager(this.CurrentNoteFile, this.Settings);
            }

            this.Renderer = new Parsing.Templater(this.Settings);
            this.RenderNotes();

        }

        private void ToggleTopmost(object sender, RoutedEventArgs e)
        {
            this.Topmost = this.OnTopMenu.IsChecked;
        }

        private void ToggleHotkeys(object sender, RoutedEventArgs e)
        {
            // Store the setting
            this.Settings.hotkeysActive = this.HotkeyToggle.IsChecked;

            if (this.Settings.hotkeysActive) { this.EnableHotkeys(); } 
            else { this.DisableHotkeys(); }
        }

        private void AdvanceSplits() { this.SplitOffset++; }
        private void ReverseSplits() { this.SplitOffset--; }

        private List<Hotkey> GetHotkeys()
        {
            return new List<Hotkey> { 
                new Hotkey(this.Settings.hotkeyNoteAdvance, this.AdvanceSplits),
                new Hotkey(this.Settings.hotkeyNoteReverse, this.ReverseSplits) 
            };
        }

        private void EnableHotkeys()
        {
            this.hotkeys.Enable(this.GetHotkeys());
        }

        private void DisableHotkeys()
        {
            this.hotkeys.Disable();
        }

        /// <summary>
        /// Create the livesplit connection and handle the loop until told to close the thread.
        /// </summary>
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
                        // Get the current index and combine with the offset, prevent from being < 0
                        int livesplitIndex = Math.Max(this.LSConnection.GetIndex() + this.SplitOffset, 0);
                        
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

        /// <summary>
        /// Open a notefile annd set it as the new set of notes for the program.
        /// </summary>
        private void OpenNotes()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Note Files (*.txt;*.md;*.html)|*.txt;*.md;*.html|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                this.CurrentNoteFile = openFileDialog.FileName;
                this.Notes = new Parsing.NoteManager(openFileDialog.FileName, this.Settings);
                this.SplitOffset = 0;  // reset the offset
                this.RenderNotes();
            }
        }

        /// <summary>
        /// Render/rerender notes with the last index value
        /// </summary>
        public void RenderNotes()
        {
            this.RenderNotes(this.LastIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="centreIndex"></param>
        public void RenderNotes(int centreIndex)
        {
            this.LastIndex = centreIndex;
            string htmlData;
            if (this.Notes != null)
            {
                
                string rawNotes = this.Notes.GetRawNotes(centreIndex);
                htmlData = this.Renderer.RenderTemplate(rawNotes);  // Feed the notes into our HTML template

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
