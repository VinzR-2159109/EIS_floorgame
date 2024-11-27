using FloorGame.Model.Calibration;
using FloorGame.Model.Player;
using Microsoft.Kinect;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FloorGame.View;

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
        int playerSize = 70;

        foreach (Player player in _playerHandler.Players)
        {
            Ellipse marker = new()
            {
                Width = playerSize,
                Height = playerSize,
                Fill = new SolidColorBrush(player.FillColor),
                Stroke = new SolidColorBrush(player.PlayerColor),
                StrokeThickness = 5,
            };
            Canvas.SetLeft(marker, player.ImagePosition.X - marker.Width / 2);
            Canvas.SetTop(marker, player.ImagePosition.Y - marker.Height / 2);
            walkableArea.Children.Add(marker);

            TextBlock textBlock = new TextBlock
            {
                Text = player.Lives.ToString(),
                Foreground = Brushes.Black,
                FontWeight = FontWeights.Bold,
                FontSize = playerSize / 3,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Width = playerSize,
                Height = playerSize,
            };
            textBlock.Padding = new Thickness(0, (playerSize - textBlock.FontSize * 1.3) / 2, 0, 0);

            Canvas.SetLeft(textBlock, player.ImagePosition.X - marker.Width / 2);
            Canvas.SetTop(textBlock, player.ImagePosition.Y - marker.Width / 2);
            walkableArea.Children.Add(textBlock);
        }
    }
}
