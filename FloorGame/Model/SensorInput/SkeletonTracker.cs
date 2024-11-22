using Microsoft.Kinect;

namespace FloorGame.Model.SensorInput;

public class SkeletonTracker
{
    private KinectSensor kinectSensor;

    public Dictionary<int, Skeleton> Skeletons { get; private set; }
    public event Action<Skeleton>? OnSkeletonUpdate;

    public SkeletonTracker(KinectSensor kinectSensor)
    {
        this.kinectSensor = kinectSensor;

        kinectSensor.SkeletonStream.Enable();
        kinectSensor.SkeletonFrameReady += KinectSensor_SkeletonFrameReady;

        Skeletons = new Dictionary<int, Skeleton>();
    }

    ~SkeletonTracker() => kinectSensor.SkeletonFrameReady -= KinectSensor_SkeletonFrameReady;

    private void KinectSensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
    {
        using SkeletonFrame skeletonFrame = e.OpenSkeletonFrame();
        if (skeletonFrame == null) return;

        Skeleton[] skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
        skeletonFrame.CopySkeletonDataTo(skeletons);

        foreach (Skeleton skeleton in skeletons)
        {
            if (skeleton.TrackingState == SkeletonTrackingState.Tracked)
            {
                Skeletons[skeleton.TrackingId] = skeleton;
                OnSkeletonUpdate?.Invoke(skeleton);
            }
        }
    }
}
