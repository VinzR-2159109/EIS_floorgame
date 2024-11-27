using System.Windows;
using Microsoft.Kinect;
using FloorGame.Model.SensorInput;

namespace FloorGame.View;

public partial class DebugWindow : Window
{
    public DebugWindow(KinectSensor kinectSensor)
    {
        InitializeComponent();

        colorImage.Source = new ColorImage(kinectSensor).Image;
        DepthImage.Source = new DepthImage(kinectSensor).Image;
        new SkeletonImage(kinectSensor, skeletonOverlayCanvas);
    }
}
