using Microsoft.Kinect;
using System.Linq;
using System.Windows;

namespace floorgame
{
    public partial class MainWindow : Window
    {
        private KinectSensor kinectSensor;
        private DebugWindow debugWindow;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize Kinect sensor once
            kinectSensor = KinectSensor.KinectSensors.FirstOrDefault(sensor => sensor.Status == KinectStatus.Connected);
            if (kinectSensor != null)
            {
                try
                {
                    kinectSensor.Start();
                }
                catch
                {
                    MessageBox.Show("Failed to start Kinect sensor.");
                    kinectSensor = null;
                    return;
                }
            }
            else
            {
                MessageBox.Show("No Kinect sensor detected.");
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Close the debug window and stop the Kinect sensor when the main window closes
            debugWindow?.Close();
            kinectSensor?.Stop();
        }

        private void OpenDebugWindow()
        {
            if (kinectSensor != null)
            {
                if (debugWindow == null || !debugWindow.IsVisible)
                {
                    // Open DebugWindow with the existing Kinect sensor
                    debugWindow = new DebugWindow(kinectSensor);
                    debugWindow.Show();
                }
                else
                {
                    // If DebugWindow is already open, bring it to the front
                    debugWindow.Activate();
                }
            }
            else
            {
                MessageBox.Show("Kinect sensor is not available.");
            }
        }

        private void OpenDebugWindow_Click(object sender, RoutedEventArgs e)
        {
            OpenDebugWindow();
        }

    }
}
