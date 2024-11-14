using System.Windows;
using floorgame.Calibration;
using System;
using System.Windows.Input;

namespace floorgame.Calibration
{
    public class CalibrationHandler
    {
        public CalibrationView calibrationView { get; private set; }
        private readonly CalibrationClass calibration;
        private Point currentCalibrationPoint;
        public event EventHandler CalibrationCompleted;

        private enum CalibrationState
        {
            TopLeft,
            TopRight,
            BottomRight,
            BottomLeft,
            Calibrate,
            MainMenu
        }

        private CalibrationState currentState = CalibrationState.TopLeft;

        public CalibrationHandler(CalibrationView view, CalibrationClass calibrationClass)
        {
            calibrationView = view;
            calibration = calibrationClass;
        }

        public Point CurrentCalibrationPoint => currentCalibrationPoint;
        public int CurrentCalibrationState => (int)currentState;

        public void Start()
        {
            UpdateCalibrationState();
        }

        private void UpdateCalibrationState()
        {
            calibrationView.ClearMarkers();
            switch (currentState)
            {
                case CalibrationState.TopLeft:
                    calibrationView.UpdateWindowTitle("Stand at the top left corner");
                    currentCalibrationPoint = new Point(0, 0);
                    calibrationView.DrawCalibrationMarker(currentCalibrationPoint);
                    break;
                case CalibrationState.TopRight:
                    calibrationView.UpdateWindowTitle("Stand at the top right corner");
                    currentCalibrationPoint = new Point(calibrationView.PlayingAreaCanvas.Width, 0);
                    calibrationView.DrawCalibrationMarker(currentCalibrationPoint);
                    break;
                case CalibrationState.BottomRight:
                    calibrationView.UpdateWindowTitle("Stand at the bottom right corner");
                    currentCalibrationPoint = new Point(calibrationView.PlayingAreaCanvas.Width, calibrationView.PlayingAreaCanvas.Height);
                    calibrationView.DrawCalibrationMarker(currentCalibrationPoint);
                    break;
                case CalibrationState.BottomLeft:
                    calibrationView.UpdateWindowTitle("Stand at the bottom left corner");
                    currentCalibrationPoint = new Point(0, calibrationView.PlayingAreaCanvas.Height);
                    calibrationView.DrawCalibrationMarker(currentCalibrationPoint);
                    break;
                case CalibrationState.Calibrate:
                    calibrationView.UpdateWindowTitle("Calibrating...");
                    calibration.Calibrate();
                    TransitionToNextState();
                    break;
                case CalibrationState.MainMenu:
                    calibrationView.UpdateWindowTitle("Main Menu");
                    CalibrationCompleted?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }

        public void TransitionToNextState()
        {
            switch (currentState)
            {
                case CalibrationState.TopLeft:
                    currentState = CalibrationState.TopRight;
                    break;
                case CalibrationState.TopRight:
                    currentState = CalibrationState.BottomRight;
                    break;
                case CalibrationState.BottomRight:
                    currentState = CalibrationState.BottomLeft;
                    break;
                case CalibrationState.BottomLeft:
                    currentState = CalibrationState.Calibrate;
                    break;
                case CalibrationState.Calibrate:
                    currentState = CalibrationState.MainMenu;
                    break;
                case CalibrationState.MainMenu:
                    return;
            }
            UpdateCalibrationState();
        }
    }
}
