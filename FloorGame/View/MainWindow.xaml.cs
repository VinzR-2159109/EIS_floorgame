using FloorGame.Model.Calibration;
using FloorGame.View;
using Microsoft.Kinect;
using System.Windows;

namespace FloorGame;

public partial class MainWindow : Window
{
    private KinectSensor _kinectSensor;
    private CalibrationPage _calibrationPage;
    private MainMenuPage? _mainMenuPage;

    public MainWindow()
    {
        InitializeComponent();

        _kinectSensor = ConnectKinectSensor();

        _calibrationPage = new CalibrationPage(_kinectSensor);
        _calibrationPage.OnCalibrated += (CalibrationClass.CalibrationData data) => { InitPages(_kinectSensor, data); _calibrationPage.Stop(); };

        mainFrame.Navigate(_calibrationPage);
    }

    private void InitPages(KinectSensor kinectSensor, CalibrationClass.CalibrationData calibrationData)
    {
        _mainMenuPage = new MainMenuPage(kinectSensor, calibrationData);
        _mainMenuPage.OnStartPongGame += () => { mainFrame.Navigate(new PongGamePage(kinectSensor, calibrationData)); _mainMenuPage.Stop(); };
        _mainMenuPage.OnStartColorMatchGame += () => { mainFrame.Navigate(new ColorMatchGamePage(kinectSensor, calibrationData)); _mainMenuPage.Stop(); };

        _mainMenuPage.Start();
        mainFrame.Navigate(_mainMenuPage);       
    }

    private KinectSensor ConnectKinectSensor()
    {
        KinectSensor kinectSensor = KinectSensor.KinectSensors.FirstOrDefault(sensor => sensor.Status == KinectStatus.Connected);
        if (kinectSensor == null)
        {
            var result = MessageBox.Show("No Kinect Sensor Detected, click on 'OK' to retry connecting to it.", "Kinect Sensor Not Found.", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Cancel) Close();
            else return ConnectKinectSensor();
        }

        kinectSensor!.Start();
        return kinectSensor;
    }

    private void OpenDebugWindow(object sender, RoutedEventArgs e)
    {
        if (_kinectSensor == null) return;
        new DebugWindow(_kinectSensor!).Show();
    }

    protected override void OnClosed(EventArgs e)
    {
        _kinectSensor?.Stop();
        base.OnClosed(e);
    }
}