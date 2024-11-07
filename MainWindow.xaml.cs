using Microsoft.Kinect;
using Microsoft.Samples.Kinect.ControlsBasics;
using System;
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

        // Enum for better state management
        private enum CalibrationState
        {
            TopLeft,
            TopRight,
            BottomRight,
            BottomLeft,
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

        private void StateMachine()
        {
            PlayingAreaCanvas.Children.Clear();
            switch (currentState)
            {
                case CalibrationState.TopLeft:
                    UpdateWindowTitle("Stand at the top left corner");
                    DrawCircle(0, 0); // Position for top left corner
                    break;
                case CalibrationState.TopRight:
                    UpdateWindowTitle("Stand at the top right corner");
                    DrawCircle(PlayingAreaCanvas.Width, 0); // Position for top right corner
                    break;
                case CalibrationState.BottomRight:
                    UpdateWindowTitle("Stand at the bottom right corner");
                    DrawCircle(PlayingAreaCanvas.Width, PlayingAreaCanvas.Height); // Position for bottom right corner
                    break;
                case CalibrationState.BottomLeft:
                    UpdateWindowTitle("Stand at the bottom left corner");
                    DrawCircle(0, PlayingAreaCanvas.Height); // Position for the bottom left corner
                    break;
                case CalibrationState.StartGame:
                    StartGame();
                    break;
            }
        }

        private void DrawCircle(double x, double y)
        {
            Ellipse circle = new Ellipse
            {
                Width = 70,
                Height = 70,
                Fill = Brushes.Yellow
            };
            Canvas.SetLeft(circle, x - circle.Width / 2);
            Canvas.SetTop(circle, y - circle.Height / 2);
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
            SkeletonPoint lastPosition = userTracker.GetLastSkeletonPosition();
            if (lastPosition != null)
            {
                try
                {
                    calibration.SetSkeletonCalibPointAtIndex(lastPosition, (int)currentState);
                    Console.WriteLine($"Saved User Position: {lastPosition.X},{lastPosition.Y},{lastPosition.Z} at index: {(int)currentState}");
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
                    currentState = CalibrationState.StartGame;
                    break;
                case CalibrationState.StartGame:
                    StartGame();
                    return; // Exit to prevent calling NextState() again
            }
            StateMachine(); // Call NextState to update UI
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
            if (e.Key == Key.F) // Check if the 'F' key is pressed
            {
                CaptureUserPosition(); // Call the function to capture the user's position
            }
        }

    }
}
