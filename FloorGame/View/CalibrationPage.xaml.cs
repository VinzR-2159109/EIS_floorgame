using System.Windows.Controls;
using System.Windows;
using FloorGame.Model.Calibration;
using Microsoft.Kinect;
using System.Windows.Shapes;
using System.Windows.Media;
using FloorGame.Model.SensorInput;
using FloorGame.Model.SensorInput.Gestures;

namespace FloorGame.View;

public partial class CalibrationPage : Page
{
    private CalibrationHandler _calibrationHandler;
    private KinectSensor _kinectSensor;

    private WaveGestureDetector _waveGestureDetector;

    public event Action<CalibrationClass.CalibrationData>? OnCalibrated;

    public CalibrationPage(KinectSensor kinectSensor)
    {
        InitializeComponent();

        _kinectSensor = kinectSensor;

        this.Focusable = true;
        this.Focus();

        _calibrationHandler = new CalibrationHandler(kinectSensor, playerArea.PlayerWalkCanvas.Width, playerArea.PlayerWalkCanvas.Height);
        _calibrationHandler.OnCalibrationStateChanged += UpdateCalibrationState;

        colorImage.Source = new ColorImage(kinectSensor).Image;
        new SkeletonImage(kinectSensor, skeletonOverlayCanvas);

        _waveGestureDetector = new WaveGestureDetector(kinectSensor);
        _waveGestureDetector.OnGestureDetected += _calibrationHandler.SetNextCalibrationPoint;

        _calibrationHandler.StartCalibration();
    }

    ~CalibrationPage()
    {
        _calibrationHandler.OnCalibrationStateChanged -= UpdateCalibrationState;
    }

    private void UpdateCalibrationState(CalibrationHandler.CalibrationState state)
    {
        playerArea.PlayerWalkCanvas.Children.Clear();
        switch (state)
        {
            case CalibrationHandler.CalibrationState.TopLeft:
                gameStateText.Text = "Stand at the top left corner, Wave to continue...";
                DrawCalibrationMarker(new Point(0, 0));
                break;
            case CalibrationHandler.CalibrationState.TopRight:
                gameStateText.Text = "Stand at the top right corner, Wave to continue";
                DrawCalibrationMarker(new Point(playerArea.PlayerWalkCanvas.Width, 0));
                break;
            case CalibrationHandler.CalibrationState.BottomRight:
                gameStateText.Text = "Stand at the bottom right corner, Wave to continue";
                DrawCalibrationMarker(new Point(playerArea.PlayerWalkCanvas.Width, playerArea.PlayerWalkCanvas.Height));
                break;
            case CalibrationHandler.CalibrationState.BottomLeft:
                gameStateText.Text = "Stand at the bottom left corner, Wave to continue";
                DrawCalibrationMarker(new Point(0, playerArea.PlayerWalkCanvas.Height));
                break;
            case CalibrationHandler.CalibrationState.Calibrate:
                gameStateText.Text = "Test the calibration by moving around, Wave to continue";
                playerArea.Init(_kinectSensor, _calibrationHandler.Calibration.Data);
                break;
            case CalibrationHandler.CalibrationState.TestCalibration:
                OnCalibrated?.Invoke(_calibrationHandler.Calibration.Data);
                break;
        }
    }

    private void DrawCalibrationMarker(Point point)
    {
        playerArea.PlayerWalkCanvas.Children.Clear();
        Ellipse marker = new()
        {
            Width = 70,
            Height = 70,
            Fill = Brushes.Yellow
        };
        Canvas.SetLeft(marker, point.X - marker.Width / 2);
        Canvas.SetTop(marker, point.Y - marker.Height / 2);
        playerArea.PlayerWalkCanvas.Children.Add(marker);
    }
}
