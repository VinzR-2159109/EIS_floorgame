using Microsoft.Kinect;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;

namespace FloorGame.Model.SensorInput;

public class SkeletonImage
{
    private KinectSensor _kinectSensor;
    private Canvas _skeletonCanvas;

    public SkeletonImage(KinectSensor kinectSensor, Canvas canvas)
    {
        _kinectSensor = kinectSensor;
        _skeletonCanvas = canvas;

        new SkeletonTracker(kinectSensor).OnSkeletonUpdate += DrawSkeleton;
    }

    public void DrawSkeleton(Skeleton skeleton)
    {
        // Clear previous skeleton drawing
        _skeletonCanvas.Children.Clear();

        // Draw joints
        foreach (Joint joint in skeleton.Joints)
        {
            if (joint.TrackingState == JointTrackingState.Tracked)
            {
                DrawJoint(joint);
            }
        }

        // Draw bones (connections between joints)
        DrawBone(skeleton, JointType.Head, JointType.ShoulderCenter);
        DrawBone(skeleton, JointType.ShoulderCenter, JointType.ShoulderLeft);
        DrawBone(skeleton, JointType.ShoulderCenter, JointType.ShoulderRight);
        DrawBone(skeleton, JointType.ShoulderCenter, JointType.Spine);

        DrawBone(skeleton, JointType.ShoulderLeft, JointType.ElbowLeft);
        DrawBone(skeleton, JointType.ShoulderRight, JointType.ElbowRight);

        DrawBone(skeleton, JointType.ElbowLeft, JointType.WristLeft);
        DrawBone(skeleton, JointType.ElbowRight, JointType.WristRight);

        DrawBone(skeleton, JointType.WristLeft, JointType.HandLeft);
        DrawBone(skeleton, JointType.WristRight, JointType.HandRight);

        DrawBone(skeleton, JointType.Spine, JointType.HipCenter);
        DrawBone(skeleton, JointType.HipCenter, JointType.HipLeft);
        DrawBone(skeleton, JointType.HipCenter, JointType.HipRight);

        DrawBone(skeleton, JointType.HipLeft, JointType.KneeLeft);
        DrawBone(skeleton, JointType.HipRight, JointType.KneeRight);

        DrawBone(skeleton, JointType.KneeLeft, JointType.FootLeft);
        DrawBone(skeleton, JointType.KneeRight, JointType.FootRight);


        // Draw Hand
        DrawHandCircle(skeleton, JointType.HandLeft);
        DrawHandCircle(skeleton, JointType.HandRight);
    }


    private void DrawJoint(Joint joint)
    {
        // Map Kinect joint to 2D depth space
        DepthImagePoint point = _kinectSensor.CoordinateMapper.MapSkeletonPointToDepthPoint(joint.Position, DepthImageFormat.Resolution640x480Fps30);

        // Scale the coordinates to the Canvas size
        double x = point.X / 640.0 * _skeletonCanvas.ActualWidth;
        double y = point.Y / 480.0 * _skeletonCanvas.ActualHeight;

        // Create and place the ellipse for the joint
        Ellipse jointEllipse = new Ellipse
        {
            Width = 10,
            Height = 10,
            Fill = Brushes.Blue
        };

        Canvas.SetLeft(jointEllipse, x - jointEllipse.Width / 2);
        Canvas.SetTop(jointEllipse, y - jointEllipse.Height / 2);
        _skeletonCanvas.Children.Add(jointEllipse);
    }

    private void DrawBone(Skeleton skeleton, JointType jointType1, JointType jointType2)
    {
        Joint joint1 = skeleton.Joints[jointType1];
        Joint joint2 = skeleton.Joints[jointType2];

        if (joint1.TrackingState == JointTrackingState.Tracked && joint2.TrackingState == JointTrackingState.Tracked)
        {
            // Map joints to 2D depth space
            DepthImagePoint point1 = _kinectSensor.CoordinateMapper.MapSkeletonPointToDepthPoint(joint1.Position, DepthImageFormat.Resolution640x480Fps30);
            DepthImagePoint point2 = _kinectSensor.CoordinateMapper.MapSkeletonPointToDepthPoint(joint2.Position, DepthImageFormat.Resolution640x480Fps30);

            // Scale the coordinates to the Canvas size
            double x1 = point1.X / 640.0 * _skeletonCanvas.ActualWidth;
            double y1 = point1.Y / 480.0 * _skeletonCanvas.ActualHeight;
            double x2 = point2.X / 640.0 * _skeletonCanvas.ActualWidth;
            double y2 = point2.Y / 480.0 * _skeletonCanvas.ActualHeight;

            // Create and place the line for the bone
            Line boneLine = new Line
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = Brushes.Red,
                StrokeThickness = 3
            };
            _skeletonCanvas.Children.Add(boneLine);
        }
    }

    private void DrawHandCircle(Skeleton skeleton, JointType handType)
    {
        Joint hand = skeleton.Joints[handType];

        if (hand.TrackingState == JointTrackingState.Tracked)
        {
            // Map the hand joint to 2D depth space
            DepthImagePoint point = _kinectSensor.CoordinateMapper.MapSkeletonPointToDepthPoint(hand.Position, DepthImageFormat.Resolution640x480Fps30);

            // Scale the coordinates to the Canvas size
            double x = point.X / 640.0 * _skeletonCanvas.ActualWidth;
            double y = point.Y / 480.0 * _skeletonCanvas.ActualHeight;

            // Create a circle around the hand
            Ellipse handCircle = new Ellipse
            {
                Width = 30,  // Circle diameter
                Height = 30,
                Stroke = Brushes.Green,  // Circle color
                StrokeThickness = 3
            };

            // Position the circle center at the hand's position
            Canvas.SetLeft(handCircle, x - handCircle.Width / 2);
            Canvas.SetTop(handCircle, y - handCircle.Height / 2);
            _skeletonCanvas.Children.Add(handCircle);
        }
    }
}
