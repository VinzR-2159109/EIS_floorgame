// Source: https://pterneas.com/2014/01/27/implementing-kinect-gestures/
using Microsoft.Kinect;
using System.Diagnostics;
using System.Windows.Threading;

namespace FloorGame.Model.SensorInput.Gestures;

public class GestureDetector
{
    public enum GesturePartResult { Failed, Succeeded, Undetermined }

    public interface IGestureSegment { GesturePartResult Update(Skeleton skeleton); }

    private IGestureSegment[] _segments;
    private int _currentSegment = 0;
    private int _frameCount = 0;
    private int _windowSize;

    private SkeletonTracker _skeletonTracker;

    public event Action<Skeleton>? OnGestureDetected;

    public GestureDetector(KinectSensor kinectSensor, int windowSize, IGestureSegment[] segments)
    {
        this._segments = segments;
        this._windowSize = windowSize;

        _skeletonTracker = new SkeletonTracker(kinectSensor);
        _skeletonTracker.OnSkeletonUpdate += Update;
    }

    public GestureDetector(KinectSensor kinectSensor, int windowSize) : this(kinectSensor, windowSize, new IGestureSegment[0]) { }

    private void Update(Skeleton skeleton)
    {
        GesturePartResult result = _segments[_currentSegment].Update(skeleton);
        if (result == GesturePartResult.Succeeded)
        {
            if (_currentSegment + 1 < _segments.Length)
            {
                _currentSegment++;
                _frameCount = 0;
            }
            else
            {
                OnGestureDetected?.Invoke(skeleton);
                Reset();
            }
        }
        else if (result == GesturePartResult.Failed || _frameCount == _windowSize)
        {
            Reset();
        }
        else
        {
            _frameCount++;
        }
    }

    private void Reset()
    {
        _currentSegment = 0;
        _frameCount = 0;
    }
}
