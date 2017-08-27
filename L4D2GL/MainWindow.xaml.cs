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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Resources;
using System.Drawing;
using System.Diagnostics;
using System.Timers;
using System.Windows.Threading;
using System.Threading;


namespace L4D2GL
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public LoaderEngine engine;
        private System.Timers.Timer time = new System.Timers.Timer();
        
        public MainWindow()
        {
            try
            {

                // Setup checker.
                time.Elapsed += new ElapsedEventHandler(DisplayTimeEvent);
                time.Interval = Constants.TimerTime;
                time.Start();

                InitializeComponent();
                engine = new LoaderEngine();
                engine.LoadSettings();
                UpdateVersionStatus();

                CheckDisclaimer();
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("Error: \n {0}", e.Message), "Error");
                throw e;
            }
        }

        public void CheckDisclaimer()
        {
            if (!engine.IsDisclaimerAccepted())
            {
                Disclaimer form = new Disclaimer();
                form.ShowDialog();
                if (form.DialogResult.HasValue && form.DialogResult.Value)
                    engine.AcceptDisclaimer();
                else
                    Close();
            }
        }

       public void DisplayTimeEvent( object source, ElapsedEventArgs e )
        {
            this.Dispatcher.BeginInvoke(
                DispatcherPriority.Send,
                (ThreadStart)delegate
                {
                    UpdateVersionStatus();
                });
       }


        private void UpdateVersionStatus()
        {
            engine.DetectVersionType();
            
            uxTypeGore.IsChecked = engine.goreType == GoreType.Gore;
            uxTypeNormal.IsChecked = engine.goreType == GoreType.Normal;
            uxTypeUnknown.IsChecked = engine.goreType == GoreType.Unknown;

            uxMessageBox.Visibility = engine.status == StatusType.None ? Visibility.Hidden : Visibility.Visible;
            uxStatusMessage.Text = engine.statusMessage;
            uxStatusImageError.Visibility = engine.status == StatusType.Error ? Visibility.Visible : Visibility.Hidden;
            uxStatusImageInfo.Visibility = engine.status == StatusType.Info ? Visibility.Visible : Visibility.Hidden;
            uxStatusImageWarning.Visibility = engine.status == StatusType.Warning ? Visibility.Visible : Visibility.Hidden;
            uxL4d2title.Content = string.Format(Constants.Left4Dead2VersionString, engine.VersionNumber);
            //uxStatusImage.Source = new BitmapImage(new Uri(string.Format("{0}{1}.png", resourcePath, engine.status.ToString())));
            //uxMessageBox.Header = engine.status.ToString();

            uxTypeGore.IsEnabled = engine.status != StatusType.Error;
            uxTypeNormal.IsEnabled = engine.status != StatusType.Error;
            uxLaunchL4D2.IsEnabled = engine.status != StatusType.Error;
        }

        private void SwitchVersion(GoreType type)
        {
            engine.SwitchVersion(type);
            UpdateVersionStatus();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void uxConfigure_Click(object sender, RoutedEventArgs e)
        {
            Configure cfg = new Configure(engine);
            cfg.ShowDialog();
            engine.LoadPaths();
            UpdateVersionStatus();
        }

        private void uxTypeGore_Checked(object sender, RoutedEventArgs e)
        {
            SwitchVersion(GoreType.Gore);
        }

        private void uxTypeNormal_Checked(object sender, RoutedEventArgs e)
        {
            SwitchVersion(GoreType.Normal);
        }

        private void uxLaunchL4D2_Click(object sender, RoutedEventArgs e)
        {
            // Check to see that nothing has changed since last checking.
            StatusType priorType = engine.status;
            UpdateVersionStatus();
            if (engine.status == priorType && engine.validPath)
            {
                Process.Start(engine.GetExePath());
                Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }


    }
}
