using AngleSharp.Dom.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SplitNotesCS
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
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
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            var settings = Properties.Settings.Default;

            int port = int.Parse(this.livesplitPort.Text);
            // Filter valid ranges for port
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
    }
}
