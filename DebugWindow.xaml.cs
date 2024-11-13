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

            // Initialize SkeletonOverlay
            skeletonOverlay = new SkeletonOverlay(SkeletonOverlayCanvas, kinectSensor);
        }

        private void DrawSkeleton(Skeleton skeleton)
        {
            skeletonOverlay.DrawSkeleton(skeleton);
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
