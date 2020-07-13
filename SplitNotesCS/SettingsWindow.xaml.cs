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


            // Validation Bindings

            // Load Values
            var settings = Properties.Settings.Default;  // Shortening the name
            this.livesplitAddress.Text = (string)settings["livesplitHostname"];
            this.livesplitPort.Text = (string)settings["livesplitPort"];
            this.splitSeparator.Text = (string)settings["splitSeparator"];
            this.previousSplits.Text = (string)settings["previousSplits"];
            this.nextSplits.Text = (string)settings["nextSplits"];
            this.fontSize.Text = (string)settings["fontSize"];
            this.textColor.Text = (string)settings["textColor"];
            this.backgroundColor.Text = (string)settings["backgroundColor"];
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            var settings = Properties.Settings.Default;

            settings["livesplitHostname"] = this.livesplitAddress.Text;
            settings["livesplitPort"] = int.Parse(this.livesplitPort.Text);
            settings["splitSeparator"] = this.splitSeparator.Text;
            settings["previousSplits"] = int.Parse(this.previousSplits.Text);
            settings["nextSplits"] = int.Parse(this.nextSplits.Text);
            settings["fontSize"] = int.Parse(this.fontSize.Text);
            settings["textColor"] = this.textColor.Text;
            settings["backgroundColor"] = this.backgroundColor.Text;

            settings.Save();
            this.Close();

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Just close the window and don't do anything
            this.Close();
        }

        private void pickTextColorButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void pickBGColorButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
