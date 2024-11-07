using System.Windows;
using Microsoft.Kinect;

namespace floorgame
{
    public partial class DebugWindow : Window
    {
        private ColorInput colorInput;
        private DepthInput depthInputViewer;

        public DebugWindow(KinectSensor kinectSensor)
        {
            InitializeComponent();

            // Initialize CameraViewer for color image
            colorInput = new ColorInput(kinectSensor);
            ColorImage.Source = colorInput.ColorImageSource;

            // Initialize DepthInputViewer for depth image
            depthInputViewer = new DepthInput(kinectSensor);
            DepthImage.Source = depthInputViewer.DepthImageSource;
        }

        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);

            // Stop both viewers when the window is closed
            colorInput?.Stop();
            depthInputViewer?.Stop();
        }
    }
}
