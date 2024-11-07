using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace floorgame
{
    public partial class DebugWindow : Window
    {
        private KinectSensor kinectSensor;
        private ColorInput colorInput;
        private DepthInput depthInputViewer;
        private TrackUserPosition userTracker;
        private SkeletonOverlay skeletonOverlay;

        public DebugWindow(KinectSensor kinectSensor)
        {
            InitializeComponent();
            this.kinectSensor = kinectSensor;

            // Initialize ColorInput for color image
            colorInput = new ColorInput(kinectSensor);
            ColorImage.Source = colorInput.ColorImageSource;

            // Initialize DepthInput for depth image
            depthInputViewer = new DepthInput(kinectSensor);
            DepthImage.Source = depthInputViewer.DepthImageSource;

            // Initialize user tracker for skeleton overlay
            TrackUserPosition userTracker = new TrackUserPosition(kinectSensor);
            userTracker.SkeletonUpdated += DrawSkeleton;
            userTracker.UserPositionUpdated += UpdateUserPosition;

            // Initialize SkeletonOverlay
            skeletonOverlay = new SkeletonOverlay(SkeletonOverlayCanvas, kinectSensor);
        }

        private void DrawSkeleton(Skeleton skeleton)
        {
            skeletonOverlay.DrawSkeleton(skeleton);
        }

        private void UpdateUserPosition(Point userPosition)
        {
            UserPositionCanvas.Children.Clear();
            // Draw a marker at the user's position
            Ellipse userMarker = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = Brushes.Red,
                Stroke = Brushes.Black
            };

            // Set position based on mapped coordinates
            Canvas.SetLeft(userMarker, userPosition.X - userMarker.Width / 2);
            Canvas.SetTop(userMarker, userPosition.Y - userMarker.Height / 2);

            UserPositionCanvas.Children.Add(userMarker);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
              
            colorInput?.Stop();
            depthInputViewer?.Stop();
            userTracker?.StopTracking();
        }
    }
}
