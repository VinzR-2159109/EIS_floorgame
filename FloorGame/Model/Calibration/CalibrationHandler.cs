using FloorGame.Model.SensorInput;
using Microsoft.Kinect;
using System.Windows;

namespace FloorGame.Model.Calibration;

public class CalibrationHandler
{
    public event Action<CalibrationState>? OnCalibrationStateChanged;

    public enum CalibrationState
    {
        Start,
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft,
        Calibrate,
        TestCalibration,
    }

    public CalibrationClass Calibration { get; private set; }
    private CalibrationState _currentState;

    private double _windowWidth, _windowHeight;

    public CalibrationHandler(KinectSensor kinectSensor, double windowWidth, double windowHeight)
    {
        Calibration = new CalibrationClass(kinectSensor);
        _currentState = CalibrationState.Start;
        _windowWidth = windowWidth;
        _windowHeight = windowHeight;
    }

    public void StartCalibration()
    {
        _currentState = CalibrationState.TopLeft;
        OnCalibrationStateChanged?.Invoke(_currentState);
    }

    public void SetNextCalibrationPoint(Skeleton skeleton)
    {
        SkeletonPoint skeletonPoint = skeleton.Position;

        switch (_currentState)
        {
            case CalibrationState.Start:
                _currentState = CalibrationState.TopLeft;
                break;
            case CalibrationState.TopLeft:
                Calibration.CaptureUserPosition(skeletonPoint, new Point(0, 0), 0);
                _currentState = CalibrationState.TopRight;
                break;
            case CalibrationState.TopRight:
                Calibration.CaptureUserPosition(skeletonPoint, new Point(_windowWidth, 0), 1);
                _currentState = CalibrationState.BottomRight;
                break;
            case CalibrationState.BottomRight:
                Calibration.CaptureUserPosition(skeletonPoint, new Point(_windowWidth, _windowHeight), 2);
                _currentState = CalibrationState.BottomLeft;
                break;
            case CalibrationState.BottomLeft:
                Calibration.CaptureUserPosition(skeletonPoint, new Point(0, _windowHeight), 3);
                Calibration.Calibrate();
                _currentState = CalibrationState.Calibrate;
                break;
            case CalibrationState.Calibrate:
                _currentState = CalibrationState.TestCalibration;
                break;
        }

        OnCalibrationStateChanged?.Invoke(_currentState);
    }
}
