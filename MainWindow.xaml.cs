using Microsoft.Kinect;
using Microsoft.Samples.Kinect.ControlsBasics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace floorgame
{
    public partial class MainWindow : Window
    {
        private KinectSensor kinectSensor;
        private DebugWindow debugWindow;
        private TrackUserPosition userTracker;
        private CalibrationClass calibration;
        private Point currentCalebPoint;
        private bool isCalibrated = false;
        private Dictionary<int, User> users = new Dictionary<int, User>();

        private static Brush[] availableColors = { Brushes.Red, Brushes.Green, Brushes.Blue, Brushes.Yellow, Brushes.Purple, Brushes.Orange };

        private enum CalibrationState
        {
            TopLeft,
            TopRight,
            BottomRight,
            BottomLeft,
            Calibrate,
            StartGame
        }

        private CalibrationState currentState = CalibrationState.TopLeft;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            KeyDown += MainWindow_KeyDown;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            kinectSensor = KinectSensor.KinectSensors.FirstOrDefault(sensor => sensor.Status == KinectStatus.Connected);
            if (kinectSensor != null)
            {
                try
                {
                    kinectSensor.Start();
                    userTracker = new TrackUserPosition(kinectSensor);
                    userTracker.SkeletonUpdated += UpdateUserPosition;
                    calibration = new CalibrationClass(kinectSensor);
                    StateMachine();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to start Kinect sensor: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("No Kinect sensor detected.");
            }
        }

        private Brush GetUserColor(int trackingId)
        {
            int colorIndex = users.Count % availableColors.Length;
            return availableColors[colorIndex];
        }

        private void UpdateUserPosition(Skeleton trackedSkeleton)
        {
            if (trackedSkeleton != null && isCalibrated)
            {
                int trackingId = trackedSkeleton.TrackingId;

                if (!users.ContainsKey(trackingId))
                {
                    users[trackingId] = new User(trackingId, GetUserColor(trackingId));
                }

                User user = users[trackingId];
                user.Position = calibration.kinectToProjectionPoint(trackedSkeleton.Position);

                user.UpdateMarkerPosition(PlayingAreaCanvas);
            }
        }

        private void StateMachine()
        {
            PlayingAreaCanvas.Children.Clear();
            switch (currentState)
            {
                case CalibrationState.TopLeft:
                    UpdateWindowTitle("Stand at the top left corner");
                    currentCalebPoint = new Point(0, 0);
                    DrawCircle();
                    break;
                case CalibrationState.TopRight:
                    UpdateWindowTitle("Stand at the top right corner");
                    currentCalebPoint = new Point(PlayingAreaCanvas.Width, 0);
                    DrawCircle();
                    break;
                case CalibrationState.BottomRight:
                    UpdateWindowTitle("Stand at the bottom right corner");
                    currentCalebPoint = new Point(PlayingAreaCanvas.Width, PlayingAreaCanvas.Height);
                    DrawCircle();
                    break;
                case CalibrationState.BottomLeft:
                    UpdateWindowTitle("Stand at the bottom left corner");
                    currentCalebPoint = new Point(0, PlayingAreaCanvas.Height);
                    DrawCircle();
                    break;
                case CalibrationState.Calibrate:
                    UpdateWindowTitle("Calibrating...");
                    calibration.Calibrate();
                    isCalibrated = true;
                    TransitionToNextState();
                    break;
                case CalibrationState.StartGame:
                    UpdateWindowTitle("Starting the game!");
                    StartGame();
                    break;
            }
        }

        private void DrawCircle()
        {
            Ellipse circle = new Ellipse
            {
                Width = 70,
                Height = 70,
                Fill = Brushes.Yellow
            };
            Canvas.SetLeft(circle, currentCalebPoint.X - circle.Width / 2);
            Canvas.SetTop(circle, currentCalebPoint.Y - circle.Height / 2);
            PlayingAreaCanvas.Children.Add(circle);
        }

        private void UpdateWindowTitle(string message)
        {
            GameState.Text = message;
        }

        private void StartGame()
        {
            UpdateWindowTitle("Starting the game!");
            // Logic to transition to the game
        }

        private void CaptureUserPosition()
        {
            SkeletonPoint skeletonPoint = userTracker.GetLastSkeletonPosition();
            if (skeletonPoint != null)
            {
                try
                {
                    calibration.SetCalibPointsAtIndex(skeletonPoint, currentCalebPoint, (int)currentState);
                    Console.WriteLine($"Saved 3D User Position: {skeletonPoint.X},{skeletonPoint.Y},{skeletonPoint.Z} at index: {(int)currentState}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    currentState = CalibrationState.StartGame;
                }
                TransitionToNextState();
            }
        }

        private void TransitionToNextState()
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
                    currentState = CalibrationState.StartGame;
                    break;
                case CalibrationState.StartGame:
                    StartGame();
                    return;
            }
            StateMachine();
        }

        private void OpenDebugWindow_Click(object sender, RoutedEventArgs e) => OpenDebugWindow();

        private void OpenDebugWindow()
        {
            if (kinectSensor != null)
            {
                if (debugWindow == null || !debugWindow.IsVisible)
                {
                    debugWindow = new DebugWindow(kinectSensor);
                    debugWindow.Show();
                }
                else
                {
                    debugWindow.Activate();
                }
            }
            else
            {
                MessageBox.Show("Kinect sensor is not available.");
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            debugWindow?.Close();
            userTracker?.StopTracking();
            kinectSensor?.Stop();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F)
            {
                CaptureUserPosition();
            }
        }
    }
}
