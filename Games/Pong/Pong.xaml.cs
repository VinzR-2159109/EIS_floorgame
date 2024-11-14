using Microsoft.Kinect;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using floorgame.Calibration;

namespace floorgame.Games
{
    public partial class Pong : UserControl
    {
        private CalibrationClass calibration;
        private TrackUserPosition userTracker;
        private DispatcherTimer gameTimer;
        private Vector ballVelocity = new Vector(5, 5);

        private int leftScore = 0;
        private int rightScore = 0;

        public Pong(KinectSensor sensor, CalibrationClass calibrationClass)
        {
            InitializeComponent();
            calibration = calibrationClass;

            userTracker = new TrackUserPosition(sensor);
            userTracker.SkeletonUpdated += UpdatePaddlePositions;

            // Initialize and start the game timer
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(30);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            this.Unloaded += OnUnloaded;
        }

        private void UpdatePaddlePositions(Skeleton trackedSkeleton)
        {
            if (trackedSkeleton != null)
            {
                SkeletonPoint spinePosition = trackedSkeleton.Joints[JointType.Spine].Position;
                Point projectedPoint = calibration.kinectToProjectionPoint(spinePosition);

                double yPosition = Math.Max(0, Math.Min(GameCanvas.ActualHeight - LeftPaddle.Height, projectedPoint.Y));
                Canvas.SetTop(LeftPaddle, yPosition);
                Canvas.SetTop(RightPaddle, yPosition);
            }
        }

        private void GameLoop(object sender, EventArgs e)
        {
            double ballX = Canvas.GetLeft(Ball) + ballVelocity.X;
            double ballY = Canvas.GetTop(Ball) + ballVelocity.Y;

            if (ballY <= 0 || ballY + Ball.Height >= GameCanvas.ActualHeight)
            {
                ballVelocity.Y = -ballVelocity.Y;
            }

            // Left paddle:
            if (ballX <= Canvas.GetLeft(LeftPaddle) + LeftPaddle.Width &&
                ballY + Ball.Height >= Canvas.GetTop(LeftPaddle) &&
                ballY <= Canvas.GetTop(LeftPaddle) + LeftPaddle.Height)
            {
                ballVelocity.X = -ballVelocity.X;
            }

            // Right paddle:
            if (ballX + Ball.Width >= Canvas.GetLeft(RightPaddle) &&
                ballY + Ball.Height >= Canvas.GetTop(RightPaddle) &&
                ballY <= Canvas.GetTop(RightPaddle) + RightPaddle.Height)
            {
                ballVelocity.X = -ballVelocity.X;
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
            ballVelocity = new Vector(ballVelocity.X, new Random().Next(-5, 5));
        }

        private void StopGame()
        {
            userTracker.StopTracking();
            gameTimer.Stop();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            StopGame();
        }
    }
}
