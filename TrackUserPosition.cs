using Microsoft.Kinect;
using Microsoft.Samples.Kinect.ControlsBasics;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;

namespace floorgame
{
    public class TrackUserPosition
    {
        private KinectSensor kinectSensor;
        private SkeletonPoint lastSkeletonPosition;
        private SkeletonPoint smoothedSkeletonPosition;
        private Point last2DPosition;
        private const float MovementThreshold = 0.05f; // Threshold for detecting actual movement (adjust as needed)
        private const float SmoothingFactor = 0.2f; // Factor for smoothing the position (0.0 to 1.0)

        public event Action<Skeleton> SkeletonUpdated;
        public event Action<SkeletonPoint> SkeletonPositionUpdated;

        public TrackUserPosition(KinectSensor sensor)
        {
            kinectSensor = sensor;

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

                Skeleton trackedSkeleton = Array.Find(skeletons, s => s.TrackingState == SkeletonTrackingState.Tracked);
                if (trackedSkeleton != null)
                {
                    SkeletonUpdated?.Invoke(trackedSkeleton);

                    if (IsMovementSignificant(trackedSkeleton.Position))
                    {
                        smoothedSkeletonPosition = SmoothPosition(trackedSkeleton.Position);
                        lastSkeletonPosition = smoothedSkeletonPosition;
                        SkeletonPositionUpdated?.Invoke(smoothedSkeletonPosition);
                    }
                }
            }
        }

        private bool IsMovementSignificant(SkeletonPoint currentPosition)
        {
            // Calculate the distance between the current position and the last position
            float distance = (float)Math.Sqrt(
                Math.Pow(currentPosition.X - lastSkeletonPosition.X, 2) +
                Math.Pow(currentPosition.Y - lastSkeletonPosition.Y, 2) +
                Math.Pow(currentPosition.Z - lastSkeletonPosition.Z, 2)
            );

            return distance >= MovementThreshold;
        }

        private SkeletonPoint SmoothPosition(SkeletonPoint currentPosition)
        {
            // Smooth the position by averaging the previous smoothed position with the current position
            smoothedSkeletonPosition.X = smoothedSkeletonPosition.X + SmoothingFactor * (currentPosition.X - smoothedSkeletonPosition.X);
            smoothedSkeletonPosition.Y = smoothedSkeletonPosition.Y + SmoothingFactor * (currentPosition.Y - smoothedSkeletonPosition.Y);
            smoothedSkeletonPosition.Z = smoothedSkeletonPosition.Z + SmoothingFactor * (currentPosition.Z - smoothedSkeletonPosition.Z);

            return smoothedSkeletonPosition;
        }

        public SkeletonPoint GetLastSkeletonPosition()
        {
            return smoothedSkeletonPosition;
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
