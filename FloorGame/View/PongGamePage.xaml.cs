using FloorGame.Model.Calibration;
using FloorGame.Model.Player;
using Microsoft.Kinect;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FloorGame.View;

public partial class PongGamePage : Page
{
    private DispatcherTimer _updateTimer;
    private PlayerHandler _playerHandler;

    private Vector _ballVelocity = new Vector(5, 5);
    private int leftScore = 0;
    private int rightScore = 0;

    public PongGamePage(KinectSensor kinectSensor, CalibrationClass.CalibrationData calibrationData)
    {
        InitializeComponent();

        _updateTimer = new DispatcherTimer();
        _updateTimer.Interval = TimeSpan.FromMilliseconds(32);
        _updateTimer.Tick += Update;

        _playerHandler = new PlayerHandler(kinectSensor, calibrationData);

        _updateTimer.Start();
    }

    ~PongGamePage()
    {
        _updateTimer.Stop();
        _updateTimer.Tick -= Update;
    }

    private void Update(object? sender, EventArgs e)
    {
        if (_playerHandler.Players.Length >= 1)
        {
            AssignPaddleControl();
        }
        GameLoop();
    }

    private void AssignPaddleControl()
    {
        Player? leftPaddlePlayer = null;
        Player? rightPaddlePlayer = null;
        double leftPaddleX = Canvas.GetLeft(LeftPaddle);
        double rightPaddleX = Canvas.GetLeft(RightPaddle);

        foreach (var player in _playerHandler.Players)
        {
            double distanceToLeft = Math.Abs(player.ImagePosition.X - leftPaddleX);
            double distanceToRight = Math.Abs(player.ImagePosition.X - rightPaddleX);

            if (leftPaddlePlayer == null || distanceToLeft < Math.Abs(leftPaddlePlayer.ImagePosition.X - leftPaddleX))
            {
                leftPaddlePlayer = player;
            }

            if (rightPaddlePlayer == null || distanceToRight < Math.Abs(rightPaddlePlayer.ImagePosition.X - rightPaddleX))
            {
                rightPaddlePlayer = player;
            }
        }

        if (leftPaddlePlayer != null)
        {
            UpdatePaddlePosition(leftPaddlePlayer.ImagePosition, isLeftPaddle: true);
        }

        if (rightPaddlePlayer != null)
        {
            UpdatePaddlePosition(rightPaddlePlayer.ImagePosition, isLeftPaddle: false);
        }
    }

    private void UpdatePaddlePosition(Point playerPoint, bool isLeftPaddle)
    {
        double yPosition = Math.Max(0, Math.Min(GameCanvas.ActualHeight - LeftPaddle.Height, playerPoint.Y));

        if (isLeftPaddle)
        {
            Canvas.SetTop(LeftPaddle, yPosition);
        }
        else
        {
            Canvas.SetTop(RightPaddle, yPosition);
        }
    }

    private void GameLoop()
    {
        double ballX = Canvas.GetLeft(Ball) + _ballVelocity.X;
        double ballY = Canvas.GetTop(Ball) + _ballVelocity.Y;

        if (ballY <= 0 || ballY + Ball.Height >= GameCanvas.ActualHeight)
        {
            _ballVelocity.Y = -_ballVelocity.Y;
        }

        const double speedIncreaseFactor = 1.05; // Factor to increase ball speed on paddle hit

        // Left paddle:
        if (ballX <= Canvas.GetLeft(LeftPaddle) + LeftPaddle.Width &&
            ballY + Ball.Height >= Canvas.GetTop(LeftPaddle) &&
            ballY <= Canvas.GetTop(LeftPaddle) + LeftPaddle.Height)
        {
            _ballVelocity.X = -_ballVelocity.X;
            _ballVelocity *= speedIncreaseFactor; // Increase speed
        }

        // Right paddle:
        if (ballX + Ball.Width >= Canvas.GetLeft(RightPaddle) &&
            ballY + Ball.Height >= Canvas.GetTop(RightPaddle) &&
            ballY <= Canvas.GetTop(RightPaddle) + RightPaddle.Height)
        {
            _ballVelocity.X = -_ballVelocity.X;
            _ballVelocity *= speedIncreaseFactor; // Increase speed
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
