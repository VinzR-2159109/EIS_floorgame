// Source: https://pterneas.com/2014/01/27/implementing-kinect-gestures/
using Microsoft.Kinect;

namespace FloorGame.Model.SensorInput.Gestures;

public class LeftWaveGestureDetector : GestureDetector
{
    private class WaveSegment1 : IGestureSegment
    {
        public GesturePartResult Update(Skeleton skeleton)
        {
            // Hand above elbow
            if (skeleton.Joints[JointType.HandLeft].Position.Y >
                skeleton.Joints[JointType.ElbowLeft].Position.Y)
            {
                // Hand right of elbow
                if (skeleton.Joints[JointType.HandLeft].Position.X >
                    skeleton.Joints[JointType.ElbowLeft].Position.X)
                {
                    return GesturePartResult.Succeeded;
                }

                return GesturePartResult.Undetermined;
            }
            // Hand dropped
            return GesturePartResult.Failed;
        }
    }

    private class WaveSegment2 : IGestureSegment
    {
        public GesturePartResult Update(Skeleton skeleton)
        {
            // Hand above elbow
            if (skeleton.Joints[JointType.HandLeft].Position.Y >
                skeleton.Joints[JointType.ElbowLeft].Position.Y)
            {
                // Hand left of elbow
                if (skeleton.Joints[JointType.HandLeft].Position.X <
                    skeleton.Joints[JointType.ElbowLeft].Position.X)
                {
                    return GesturePartResult.Succeeded;
                }

                return GesturePartResult.Undetermined;
            }
            // Hand dropped
            return GesturePartResult.Failed;
        }
    }

    public LeftWaveGestureDetector(KinectSensor kinectSensor) : base(
        kinectSensor,
        50,
        new IGestureSegment[] { 
            new WaveSegment1(), 
            new WaveSegment2(), 
            new WaveSegment1(), 
            new WaveSegment2(), 
            new WaveSegment1(), 
            new WaveSegment2() 
    }) { }
}
