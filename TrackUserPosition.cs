using Microsoft.Kinect;
using Microsoft.Samples.Kinect.ControlsBasics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;

namespace floorgame
{
    public class TrackUserPosition
    {
        private KinectSensor kinectSensor;
        private CalibrationClass calibrationClass;

        // Store the smoothed user position
        private Queue<Point> positionHistory = new Queue<Point>();
        private const int historySize = 5; // Number of samples to average
        private const double movementThreshold = 15; // Movement threshold to reduce flickering

        private SkeletonPoint lastSkeletonPositon;

        public event Action<Skeleton> SkeletonUpdated;
        public event Action<Point> UserPositionUpdated;

        public TrackUserPosition(KinectSensor sensor)
        {
            kinectSensor = sensor;
            calibrationClass = new CalibrationClass(sensor);

            // Enable skeleton tracking
            if (kinectSensor != null)
            {
                kinectSensor.SkeletonStream.Enable();
                kinectSensor.SkeletonFrameReady += KinectSensor_SkeletonFrameReady;
            }
        }

        private void KinectSensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame == null) return;

                Skeleton[] skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                skeletonFrame.CopySkeletonDataTo(skeletons);

                // Find the first tracked skeleton
                Skeleton trackedSkeleton = Array.Find(skeletons, s => s.TrackingState == SkeletonTrackingState.Tracked);
                if (trackedSkeleton != null)
                {
                    // Trigger event with the full skeleton data
                    SkeletonUpdated?.Invoke(trackedSkeleton);
                    lastSkeletonPositon = trackedSkeleton.Position;

                    // Map the position of the center joint (HipCenter) to screen coordinates
                    Point userScreenPosition = MapSkeletonToScreen(trackedSkeleton);

                    // Update user position if it's valid
                    if (userScreenPosition != default)
                    {
                        UpdateUserPosition(userScreenPosition);
                    }
                }
            }
        }

        public Point MapSkeletonToScreen(Skeleton skeleton)
        {
            Joint body = skeleton.Joints[JointType.HipCenter];
            if (body.TrackingState == JointTrackingState.Tracked)
            {
                // Map the body joint to depth point
                Point3D depthPoint = calibrationClass.convertSkeletonPointToDepthPoint(skeleton.Position);

                if (ScreenParameters.ScreenHeight > 0 && ScreenParameters.ScreenWidth > 0)
                {
                    double screenY = (depthPoint.Y / 480.0) * ScreenParameters.ScreenHeight;
                    double screenX = (depthPoint.X / 640.0) * ScreenParameters.ScreenWidth;

                    return new Point(screenX, screenY);
                }
            }

            return new Point(0, 0); // Return default point if body is not tracked
        }

        private Point SmoothPosition(Point newPosition)
        {
            if (positionHistory.Count >= historySize)
            {
                positionHistory.Dequeue(); // Remove the oldest position
            }

            positionHistory.Enqueue(newPosition); // Add the new position

            // Calculate the average position
            double avgX = positionHistory.Average(p => p.X);
            double avgY = positionHistory.Average(p => p.Y);

            return new Point(avgX, avgY);
        }

        public void UpdateUserPosition(Point userPosition)
        {
            // Check if the movement exceeds the threshold
            if (positionHistory.Count > 0)
            {
                Point currentMarkerPosition = SmoothPosition(positionHistory.Last());

                if (Math.Abs(userPosition.X - currentMarkerPosition.X) < movementThreshold &&
                    Math.Abs(userPosition.Y - currentMarkerPosition.Y) < movementThreshold)
                {
                    return; // Ignore small movements
                }
            }

            // Smooth the user position
            Point smoothedPosition = SmoothPosition(userPosition);

            // Trigger the user position update event
            UserPositionUpdated?.Invoke(smoothedPosition);
        }

        public SkeletonPoint GetLastSkeletonPosition()
        {
            return lastSkeletonPositon;
        }

        public void StopTracking()
        {
            if (kinectSensor != null)
            {
                kinectSensor.SkeletonFrameReady -= KinectSensor_SkeletonFrameReady;
            }
        }
    }
}
