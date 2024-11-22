using FloorGame.Model.Calibration;
using FloorGame.Model.Player;
using Microsoft.Kinect;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FloorGame.View;

/// <summary>
/// Interaction logic for WalkableAreaUserControl.xaml
/// </summary>
public partial class WalkableAreaUserControl : UserControl
{
    private DispatcherTimer _updateTimer;
    private PlayerHandler? _playerHandler;

    public Canvas PlayerWalkCanvas { get; private set; }
    public PlayerHandler PlayerHandler => _playerHandler!;

    public WalkableAreaUserControl()
    {
        InitializeComponent();

        _updateTimer = new DispatcherTimer();
        _updateTimer.Interval = TimeSpan.FromMilliseconds(32);
        _updateTimer.Tick += Update;

        PlayerWalkCanvas = walkableArea;
    }

    ~WalkableAreaUserControl()
    {
        _updateTimer.Stop();
        _updateTimer.Tick -= Update;
    }

    public void Init(KinectSensor kinectSensor, CalibrationClass.CalibrationData calibrationData)
    {
        _playerHandler = new PlayerHandler(kinectSensor, calibrationData);
        _updateTimer.Start();
    }

    private void Update(object? sender, EventArgs e)
    {
        if (_playerHandler == null) return;

        walkableArea.Children.Clear();

        foreach (Player player in _playerHandler.Players)
        {
            Ellipse marker = new()
            {
                Width = 70,
                Height = 70,
                Fill = Brushes.Yellow
            };
            Canvas.SetLeft(marker, player.ImagePosition.X - marker.Width / 2);
            Canvas.SetTop(marker, player.ImagePosition.Y - marker.Height / 2);
            walkableArea.Children.Add(marker);
        }
    }
}
