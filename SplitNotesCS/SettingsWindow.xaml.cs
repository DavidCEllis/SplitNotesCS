using LowLevelHooking;
using SplitNotesCS.Hotkeys;
using System;
using System.Windows;

namespace SplitNotesCS
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        // Needed for configuration

        private readonly HotkeyManager hotkeyManager;

        private VirtualKey nextKey;
        private VirtualKey previousKey;

        public SettingsWindow(HotkeyManager hotkeyManager)
        {
            InitializeComponent();

            this.hotkeyManager = hotkeyManager;
            this.Loaded += this.OnPageLoaded;  // Subscribe to the loaded event
        }

        private void OnPageLoaded(object sender, EventArgs e)
        {
            // Validation Bindings (Not done yet)

            // Load Values
            var settings = Properties.Settings.Default;  // Shortening the name
            this.livesplitAddress.Text = settings.livesplitHostname;
            this.livesplitPort.Text = settings.livesplitPort.ToString();
            this.splitSeparator.Text = settings.splitSeparator;
            this.previousSplits.Text = settings.previousSplits.ToString();
            this.nextSplits.Text = settings.nextSplits.ToString();
            this.fontSize.Text = settings.fontSize.ToString();
            this.textColor.Text = settings.textColor;
            this.backgroundColor.Text = settings.backgroundColor;

            this.nextKey = (VirtualKey)settings.hotkeyNoteAdvance;
            this.previousKey = (VirtualKey)settings.hotkeyNoteReverse;

            this.nextSplitHotkey.Text = (this.nextKey == VirtualKey.Escape) ? "Unbound" : this.nextKey.ToString();
            this.previousSplitHotkey.Text = (this.previousKey == VirtualKey.Escape) ? "Unbound" : this.previousKey.ToString();

        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var settings = Properties.Settings.Default;

            int port = int.Parse(this.livesplitPort.Text);
            // Filter valid ranges for port - so far we'll just ignore invalid values
            if (port >= 1 && port <= 65535)
            {
                settings.livesplitPort = port;
            }


            settings.livesplitHostname = this.livesplitAddress.Text;

            settings.splitSeparator = this.splitSeparator.Text;
            settings.previousSplits = Math.Abs(int.Parse(this.previousSplits.Text));
            settings.nextSplits = Math.Abs(int.Parse(this.nextSplits.Text));
            settings.fontSize = Math.Abs(int.Parse(this.fontSize.Text));
            settings.textColor = this.textColor.Text;
            settings.backgroundColor = this.backgroundColor.Text;

            settings.hotkeyNoteAdvance = (int)this.nextKey;
            settings.hotkeyNoteReverse = (int)this.previousKey;

            settings.Save();
            this.Close();

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Just close the window and don't do anything
            this.Close();
        }

        private void PickTextColorButton_Click(object sender, RoutedEventArgs e)
        {
            // Open a color form
            var colorPicker = new System.Windows.Forms.ColorDialog();
            if (colorPicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var result = System.Drawing.ColorTranslator.ToHtml(colorPicker.Color);
                this.textColor.Text = result;
            }
        }

        private void PickBGColorButton_Click(object sender, RoutedEventArgs e)
        {
            // Open a color form
            var colorPicker = new System.Windows.Forms.ColorDialog();
            if (colorPicker.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var result = System.Drawing.ColorTranslator.ToHtml(colorPicker.Color);
                this.backgroundColor.Text = result;
            }
        }

        private void nextSplitHotkeySelect_Click(object sender, RoutedEventArgs e)
        {
            this.nextSplitHotkeySelect.IsEnabled = false;
            this.previousSplitHotkeySelect.IsEnabled = false; // Also disable the other select button
            this.nextSplitHotkeySelect.Content = "Listening...";
            this.hotkeyManager.ChooseKey(setNextSplitKey);
        }

        public void setNextSplitKey(int keycode)
        {
            this.nextKey = (VirtualKey)keycode;
            this.nextSplitHotkey.Text = (this.nextKey == VirtualKey.Escape) ? "Unbound" : this.nextKey.ToString();
            this.nextSplitHotkeySelect.IsEnabled = true;
            this.nextSplitHotkeySelect.Content = "Pick";

            this.previousSplitHotkeySelect.IsEnabled = true;  // Enable the other control again
        }

        private void previousSplitHotkeySelect_Click(object sender, RoutedEventArgs e)
        {
            this.nextSplitHotkeySelect.IsEnabled = false;  // Also disable the other select button
            this.previousSplitHotkeySelect.IsEnabled = false;
            this.previousSplitHotkeySelect.Content = "Listening...";
            this.hotkeyManager.ChooseKey(setPreviousSplitKey);
        }

        public void setPreviousSplitKey(int keycode)
        {
            this.previousKey = (VirtualKey)keycode;
            this.previousSplitHotkey.Text = (this.previousKey == VirtualKey.Escape) ? "Unbound" : this.previousKey.ToString();
            this.previousSplitHotkeySelect.IsEnabled = true;
            this.previousSplitHotkeySelect.Content = "Pick";

            this.nextSplitHotkeySelect.IsEnabled = true;  // Enable the other control again
        }

    }
}
