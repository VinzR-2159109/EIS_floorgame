using Microsoft.Kinect;
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

        public event Action<Skeleton> SkeletonUpdated;

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

                foreach (Skeleton skeleton in skeletons)
                {
                    if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        lastSkeletonPosition = skeleton.Position;
                        SkeletonUpdated?.Invoke(skeleton);
                    }
                }
            }
        }

        public SkeletonPoint GetLastSkeletonPosition()
        {
            return lastSkeletonPosition;
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
