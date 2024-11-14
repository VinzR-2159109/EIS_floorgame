using Microsoft.Kinect;
using floorgame.Calibration;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using floorgame.Games;

namespace floorgame
{
    public partial class MainWindow : Window
    {
        private KinectSensor kinectSensor;
        private DebugWindow debugWindow;
        private TrackUserPosition userTracker;
        private CalibrationClass calibration;
        private CalibrationHandler calibrationHandler;
        private UserHandler userHandler;
        private MainMenu mainMenu;

        public MainWindow()
        {
            kinectSensor = KinectSensor.KinectSensors.FirstOrDefault(sensor => sensor.Status == KinectStatus.Connected);
            if (kinectSensor != null)
            {
                this.userTracker = new TrackUserPosition(kinectSensor);
                kinectSensor.Start();
            }
            else
            {
                MessageBox.Show("No Kinect sensor detected.");
            }

            InitializeComponent();
            ShowCalibrationView();

            userHandler = new UserHandler();

            Closing += MainWindow_Closing;
            KeyDown += MainWindow_KeyDown;
        }


        public void ShowCalibrationView()
        {
            if (kinectSensor != null)
            {
                calibration = new CalibrationClass(kinectSensor);
                CalibrationView calibrationView = new CalibrationView();
                calibrationHandler = new CalibrationHandler(calibrationView, calibration);

                calibrationHandler.CalibrationCompleted += OnCalibrationCompleted;

                MainContent.Content = calibrationView;
                calibrationHandler.Start();
            }
        }

        public void ShowMainMenu()
        {
            mainMenu = new MainMenu(calibration, userHandler, userTracker);
            MainContent.Content = mainMenu;
            mainMenu.StartPongGame += OnStartPongGame;
        }

        private void OnStartPongGame(object sender, EventArgs e)
        {
            MainContent.Content = new Pong(kinectSensor, calibration);
        }

        private void OnCalibrationCompleted(object sender, EventArgs e)
        {
            ShowMainMenu();
        }


        private void OpenDebugWindow_Click(object sender, RoutedEventArgs e)
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
                calibration.CaptureUserPosition(userTracker.GetLastSkeletonPosition(), calibrationHandler.CurrentCalibrationPoint, calibrationHandler.CurrentCalibrationState);
                calibrationHandler.TransitionToNextState();
            }
        }
    }
}
