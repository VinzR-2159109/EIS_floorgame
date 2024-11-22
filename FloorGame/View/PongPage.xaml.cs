using FloorGame.Model.Calibration;
using FloorGame.Model.Player;
using Microsoft.Kinect;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FloorGame.View;

public partial class PongPage : Page
{
    private DispatcherTimer _updateTimer;
    private PlayerHandler _playerHandler;

    private Vector _ballVelocity = new Vector(5, 5);
    private int leftScore = 0;
    private int rightScore = 0;

    public PongPage(KinectSensor kinectSensor, CalibrationClass.CalibrationData calibrationData)
    {
        InitializeComponent();

        _updateTimer = new DispatcherTimer();
        _updateTimer.Interval = TimeSpan.FromMilliseconds(32);
        _updateTimer.Tick += Update;

        _playerHandler = new PlayerHandler(kinectSensor, calibrationData);

        _updateTimer.Start();
    }

    ~PongPage()
    {
        _updateTimer.Stop();
        _updateTimer.Tick -= Update;
    }

    private void Update(object? sender, EventArgs e)
    {
        foreach (Player player in _playerHandler.Players) UpdatePaddlePositions(player.ImagePosition);
        GameLoop();
    }

    private void UpdatePaddlePositions(Point playerPoint)
    {
        double yPosition = Math.Max(0, Math.Min(GameCanvas.ActualHeight - LeftPaddle.Height, playerPoint.Y));
        Canvas.SetTop(LeftPaddle, yPosition);
        Canvas.SetTop(RightPaddle, yPosition);
    }

    private void GameLoop()
    {
        double ballX = Canvas.GetLeft(Ball) + _ballVelocity.X;
        double ballY = Canvas.GetTop(Ball) + _ballVelocity.Y;

        if (ballY <= 0 || ballY + Ball.Height >= GameCanvas.ActualHeight)
        {
            _ballVelocity.Y = -_ballVelocity.Y;
        }

        // Left paddle:
        if (ballX <= Canvas.GetLeft(LeftPaddle) + LeftPaddle.Width &&
            ballY + Ball.Height >= Canvas.GetTop(LeftPaddle) &&
            ballY <= Canvas.GetTop(LeftPaddle) + LeftPaddle.Height)
        {
            _ballVelocity.X = -_ballVelocity.X;
        }

        // Right paddle:
        if (ballX + Ball.Width >= Canvas.GetLeft(RightPaddle) &&
            ballY + Ball.Height >= Canvas.GetTop(RightPaddle) &&
            ballY <= Canvas.GetTop(RightPaddle) + RightPaddle.Height)
        {
            _ballVelocity.X = -_ballVelocity.X;
        }

        Canvas.SetLeft(Ball, ballX);
        Canvas.SetTop(Ball, ballY);

        if (ballX < 0) // Right player scores
        {
            rightScore++;
            RightScore.Text = rightScore.ToString();
            ResetBall();
        }
        else if (ballX + Ball.Width > GameCanvas.ActualWidth) // Left player scores
        {
            leftScore++;
            LeftScore.Text = leftScore.ToString();
            ResetBall();
        }
    }

    private void ResetBall()
    {
        Canvas.SetLeft(Ball, GameCanvas.ActualWidth / 2 - Ball.Width / 2);
        Canvas.SetTop(Ball, GameCanvas.ActualHeight / 2 - Ball.Height / 2);
        _ballVelocity = new Vector(_ballVelocity.X, new Random().Next(-5, 5));
    }
}
