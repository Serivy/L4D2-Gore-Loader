using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;

namespace L4D2GL
{
    /// <summary>
    /// Interaction logic for Configure.xaml
    /// </summary>
    public partial class Configure : Window
    {
        private LoaderEngine engine;
        private string pathToSave = null;
        public Configure(LoaderEngine currentEngine)
        {
            if (currentEngine == null)
                throw new ArgumentNullException("currentEngine");

            engine = currentEngine;
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            pathToSave = engine.GetManualPath();
            UpdateUI();
        }

        /// <summary>
        /// Update the user interface controls based on the current state variables.
        /// </summary>
        private void UpdateUI()
        {
            var displayPath = Constants.ConfigUIBadPath;

            uxAutoPath.IsChecked = (pathToSave == null);
            uxManualPath.IsChecked = (pathToSave != null);

            if (pathToSave == null)
                displayPath = engine.GetAutoPath();
            else
                displayPath = pathToSave;

            if (displayPath == null)
                displayPath = Constants.ConfigUIBadPath;

            uxTxtL4D2Directory.Text = displayPath;
        }

        private void uxOk_Click(object sender, RoutedEventArgs e)
        {
            // Update with the path to be saved.
            engine.SetManualPath(pathToSave);
            DialogResult = true;
            this.Close();
        }

        private void uxCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void uxOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            if (System.IO.Directory.Exists(uxTxtL4D2Directory.Text))
                Process.Start("explorer.exe", uxTxtL4D2Directory.Text);
        }


        /// <summary>
        /// When the auto detect 
        /// </summary>
        private void uxAutoPath_Checked(object sender, RoutedEventArgs e)
        {
            pathToSave = null;
            UpdateUI();
        }

        private void uxManualPath_Checked(object sender, RoutedEventArgs e)
        {
            if (uxTxtL4D2Directory.Text != string.Empty) // Check to ensure that it isn't changed by the fired event.
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    if (engine.ValidGamePath(dialog.SelectedPath.ToString()))
                        pathToSave = dialog.SelectedPath.ToString();
                    else
                        MessageBox.Show(Constants.ConfigBadPathMessage, Constants.ConfigBadPathCaption, MessageBoxButton.OK, MessageBoxImage.Error);
                }

                UpdateUI();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you wish to reset all the settings?", "Reset?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                engine.SetManualPath(null);
                engine.DeleteDisclaimer();
                Application.Current.Shutdown();
            }
        }

        private void label2_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(Constants.ProjectURL);
        }

    }
}
